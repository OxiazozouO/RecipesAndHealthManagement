// 食材卡片模板
function createIngredientCard(ingredient) {
    const card = document.createElement('div');
    card.className = 'ingredient-card';
    card.dataset.name = ingredient.iName
    card.dataset.ingredientId = ingredient.ingredientId
    let isExpanded = false;

    // 严格顺序定义
    const priorityOrder = ['热量 kcal', '蛋白质', '脂肪', '碳水化合物'];

    // 按顺序提取优先元素
    const priorityNutrients = priorityOrder
        .map(name =>
            Object.values(ingredient.nutritional)
                .find(n => n.name === name)
        )
        .filter(Boolean);

    // 其他营养元素
    const otherNutrients = Object.values(ingredient.nutritional)
        .filter(n => !priorityOrder.includes(n.name));

    // 生成营养元素HTML
    const generateNutrientHTML = (nutrient, isPriority, shouldHide = false) => {
        const calculatedValue = (nutrient.value * ingredient.dosage / 100).toFixed(2);
        return `
            <span class="${isPriority ? 'priority-nutrient' : 'normal-nutrient'}
                         ${shouldHide ? 'hidden-nutrient' : ''}"
                  data-id="${nutrient.id}"
                  data-original-value="${nutrient.value}"
                  data-name="${nutrient.name}"
                  data-unit="${nutrient.unit}"
                  style="${isPriority ? 'order: -1;' : ''}">
                ${nutrient.name}: ${calculatedValue}${nutrient.unit}
            </span>`;
    };

    // 生成优先元素
    const priorityHTML = priorityNutrients.map(n => {
        const isCalorie = n.name === '热量 kcal';
        return generateNutrientHTML(n, true)
            .replace('priority-nutrient', isCalorie ? 'calorie-nutrient' : 'sub-priority-nutrient');
    }).join('');

    // 生成其他元素（带初始隐藏状态）
    const otherHTML = otherNutrients
        .map((n, index) =>
            generateNutrientHTML(n, false, !isExpanded && index >= 3)
        ).join('');

    card.innerHTML = `
        <img src="${ingredient.fileUrl}" class="ingredient-img"  onclick="previewImg(this)">
        <div class="ingredient-info">
            <div style="margin-bottom: 5px">
                <strong>${ingredient.iName}</strong>
            </div>
            <div class="nutritional-info" style="display: flex; flex-wrap: wrap; gap: 10px; align-items: center;">
                ${priorityHTML}
                ${otherHTML}
            </div>
            ${otherNutrients.length > 3 ?
        `<button type="button" class="toggle-nutrients" style="margin-top:10px; padding:2px 8px; border:1px solid #ddd; background:none; cursor:pointer;">
                    ${isExpanded ? '收起' : '展开全部'}
                </button>` : ''}
        </div>
        <div class="layui-input-inline">
            <input type="number"
                   data-id="${ingredient.ingredientId}"
                   class="layui-input dosage-input"
                   value="${ingredient.dosage}"
                   min="0"
                   step="1"
                   placeholder="用量">
        </div>
        <span class="layui-badge layui-bg-gray">${ingredient.unit}</span>
        <span class="del-ingredient"><i class="iconfont">&#xe630;</i> 删除</span>`;

    // 展开/收起功能
    const toggleBtn = card.querySelector('.toggle-nutrients');
    if (toggleBtn) {
        toggleBtn.addEventListener('click', (e) => {
            e.preventDefault();
            isExpanded = !isExpanded;

            // 精确控制其他元素的显示
            const otherElements = Array.from(card.querySelectorAll('.normal-nutrient'));
            otherElements.forEach((el, index) => {
                if (index >= 3) {
                    el.classList.toggle('hidden-nutrient', !isExpanded);
                }
            });

            // 更新按钮文字
            toggleBtn.textContent = isExpanded ? '收起' : '展开全部';

            // 强制重绘避免布局异常
            card.querySelector('.nutritional-info').offsetHeight;
        });
    }

    // 剂量变更监听
    const dosageInput = card.querySelector('.dosage-input');
    dosageInput.addEventListener('input', function () {
        const newDosage = parseFloat(this.value) || 0;
        card.querySelectorAll('[data-original-value]').forEach(item => {
            const newValue = (parseFloat(item.dataset.originalValue) * newDosage) / 100;
            item.textContent = `${item.dataset.name}: ${newValue.toFixed(2)}${item.dataset.unit}`;
        });
        updateNutritionOverview()
    });

    return card;
}


// 更新营养总览
function updateNutritionOverview() {
    const totals = {
        '热量 kcal': {value: 0, unit: 'kcal'},
        '蛋白质': {value: 0, unit: 'g'},
        '脂肪': {value: 0, unit: 'g'},
        '碳水化合物': {value: 0, unit: 'g'}
    }

    // 遍历所有食材卡片
    document.querySelectorAll('.ingredient-card').forEach(card => {
        const dosage = parseFloat(card.querySelector('.dosage-input').value) || 0;
        if (dosage <= 0) return;

        // 获取营养成分数据
        const nutrients = {};
        card.querySelectorAll('[data-name]').forEach(item => {
            nutrients[item.dataset.name] = {
                value: parseFloat(item.dataset.originalValue),
                unit: item.dataset.unit
            };
        });

        // 累加计算
        Object.entries(totals).forEach(([name, total]) => {
            if (nutrients[name] && nutrients[name].value) {
                total.value += (nutrients[name].value * dosage) / 100;
            }
        });
    });

    // 生成HTML
    const hasData = Object.values(totals).some(t => t.value > 0);
    const overview = document.getElementById('nutritionOverview');
    overview.style.display = hasData ? 'flex' : 'none';

    overview.innerHTML = hasData
        ? Object.entries(totals)
            .map(([name, data]) => `
                <div class="nutrition-item">
                    <span class="nutrition-label">${name}</span>
                    <span class="nutrition-value">${data.value.toFixed(2)}</span>
                    <span class="nutrition-unit">${data.unit}</span>
                </div>`)
            .join('')
        : '<div class="no-nutrition">暂无营养数据</div>';
}


function getIngredients(ingredientContainer, isName) {
    const ingredients = [];
    ingredientContainer.querySelectorAll('.ingredient-card').forEach(card => {
        const dosageInput = card.querySelector('.dosage-input');
        ingredients.push(isName ? {
                    ingredientId: parseInt(dosageInput.dataset.id),
                    iName: card.dataset.name,
                    dosage: parseInt(dosageInput.value) || 0
                }
                : {
                    ingredientId: parseInt(dosageInput.dataset.id),
                    dosage: parseInt(dosageInput.value) || 0
                }
        );
    })
    return ingredients
}