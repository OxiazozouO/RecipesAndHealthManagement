function initBody() {
    document.body.innerHTML = `
<div class="layui-fluid">
    <div class="layui-row">
        <form class="layui-form">
            <!-- 食谱主信息 -->
            <div class="layui-form-item" id="m-upload"></div>

            <div class="layui-form-item">
                <label class="layui-form-label"><span class="x-red">*</span>食谱标题</label>
                <div class="layui-input-block">
                    <input type="text" name="title" id="title" required lay-verify="required" class="layui-input"
                           placeholder="请输入食谱标题">
                </div>
            </div>

            <div class="layui-form-item">
                <label class="layui-form-label">食谱名称</label>
                <div class="layui-input-block">
                    <input type="text" name="rname" id="rname" class="layui-input" placeholder="请输入食谱名称">
                </div>
            </div>

            <div class="layui-form-item">
                <label class="layui-form-label">简介</label>
                <div class="layui-input-block">
                    <textarea name="summary" id="summary" class="layui-textarea"
                              placeholder="请输入食谱简介"></textarea>
                </div>
            </div>

            <!-- 步骤编辑 -->
            <div class="layui-form-item">
                <div class="layui-form-label">制作步骤</div>
                <div class="layui-input-block">
                    <button type="button" class="layui-btn layui-btn-sm" id="toggleAllSteps">
                        <i class="iconfont">&#xe65a;</i>全部收起
                    </button>
                    <div id="stepsContainer"></div>
                    <button type="button" class="layui-btn layui-btn-sm" id="addStep">
                        <i class="layui-icon">&#xe654;</i>添加步骤
                    </button>
                </div>
            </div>

            <!-- 使用食材部分 -->
            <div class="layui-form-item">
                <label class="layui-form-label">使用食材</label>
                <div class="layui-input-block">
                    <div id="ingredientContainer" class="nutrient-container"></div>
                    <div id="nutritionOverview" class="nutrition-total" style="display: none;">
                        <div class="no-nutrition">暂无营养数据</div>
                    </div>
                    <div style="margin-top: 10px">
                        <button type="button" class="layui-btn layui-btn-sm" id="addIngredient">
                            <i class="layui-icon">&#xe654;</i>添加食材
                        </button>
                    </div>
                </div>
            </div>


            <div class="layui-form-item">
                <label class="layui-form-label"></label>
                <button class="layui-btn" lay-filter="submit" lay-submit>提交</button>
            </div>
        </form>
    </div>
</div>`
}

var aid = sessionStorage.getItem('aid')
var releaseId = -1
var apiUrl = null
var recipe = null;

initBody()
const ui = {
    stepsContainer: document.getElementById('stepsContainer'),
    toggleAllSteps: document.getElementById('toggleAllSteps'),
    addStep: document.getElementById('addStep'),
    imgUi: buildImg('m-upload', '封面图', '点击上传封面图'),
    title: document.getElementById('title'),
    rname: document.getElementById('rname'),
    summary: document.getElementById('summary'),
    ingredientContainer: document.getElementById('ingredientContainer'),
    addIngredient: document.getElementById('addIngredient')
}

ui.stepsContainer.uis = []

// 批量折叠/展开控制
let isAllCollapsed = true;
ui.toggleAllSteps.addEventListener('click', () => {
    isAllCollapsed = !isAllCollapsed;
    if (isAllCollapsed) {
        ui.toggleAllSteps.innerHTML = `<i class="iconfont">&#xe65a;</i>全部收起`
    } else {
        ui.toggleAllSteps.innerHTML = `<i class="iconfont">&#xe659;</i>全部展开`
    }
    const steps = document.querySelectorAll('.step-card');
    steps.forEach(step => {
        step.classList.toggle('collapsed', !isAllCollapsed);
        step.querySelector('.toggle-step').innerHTML = isAllCollapsed ? '<i class="iconfont">&#xe65a;</i>'
            : '<i class="iconfont">&#xe659;</i>';
    });

    this.querySelector('i').className = `layui-icon ${isAllCollapsed ? '&#xe61a;' : '&#xe619;'}`;
    this.innerHTML = this.innerHTML.replace(/全部(展开|折叠)/, `全部${isAllCollapsed ? '展开' : '折叠'}`);
});

function init() {
    ui.imgUi.init(recipe.fileUrl)
    ui.title.value = recipe.title
    ui.rname.value = recipe.rName
    ui.summary.value = recipe.summary

    initIngredients(recipe.ingredients)
    initSteps(recipe.steps)

    updateSteps(ui.stepsContainer, getIngredients(ui.ingredientContainer, true))
    updateNutritionOverview()
}


function initIngredients(ingredients) {
    const i = getIngredients(ui.ingredientContainer, true);
    const existingIds = new Set(i.map(iii => iii.ingredientId));
    ingredients = ingredients.filter(ii => !existingIds.has(ii.ingredientId));

    ingredients.forEach(ingredient => {
        const ingredientCard = createIngredientCard(ingredient)
        ui.ingredientContainer.appendChild(ingredientCard)
        // 删除功能
        ingredientCard.querySelector('.del-ingredient').addEventListener('click', () => {
            ingredientCard.remove()
            updateSteps(ui.stepsContainer, getIngredients(ui.ingredientContainer, true))
            updateNutritionOverview()
        });
    })
}

function initSteps(steps) {
    steps.forEach(step => createStep(ui.stepsContainer, step))
}

initLay((form, upload, layer) => {
    myUpload(upload, layer, ui.imgUi, httpTool.fileUpload)
    ui.stepsContainer.addUi = i => myUpload(upload, layer, i.imgUi, httpTool.fileUpload)
    // 表单提交处理
    form.on('submit(submit)', data => {
        const steps = getSteps(ui.stepsContainer)
        const ingredients = getIngredients(ui.ingredientContainer, false)
        if (ingredients.length === 0) {
            layer.alert('请添加至少一个食材', {icon: 5})
            return false
        }

        const req = {
            AdminId: aid,
            ReleaseId: releaseId,
            Recipe: {
                recipeId: recipe?.recipeId || -1,
                fileUrl: ui.imgUi.fileUrl.value,
                title: ui.title.value,
                rName: ui.rname.value,
                summary: ui.summary.value,
                ingredients: ingredients,
                steps: steps,
                categories: recipe?.categories || [],
            }
        }

        httpTool.execute(apiUrl, req).then(res => {
            if (res === undefined) return
            layer.alert(res.message, {icon: 6}, () => {
                parent.layer.close(parent.layer.getFrameIndex(window.name))
            })
        })
        return false;
    })
})

ui.addStep.addEventListener('click', () => {
    createStep(ui.stepsContainer)
    updateSteps(ui.stepsContainer, getIngredients(ui.ingredientContainer, true))
    updateNutritionOverview()
})
ui.addIngredient.addEventListener('click', () => {
    const ids = Array.from(ui.ingredientContainer.children).map(i => i.dataset.ingredientId)
    sessionStorage.setItem('select-flag', 'ingredient')
    sessionStorage.setItem('ids', ids)
    xadmin.open('搜索食材', '../../comm/list-select/list-select.html', '', '', true)
})

window.addEventListener('message', (event) => {
    const data = JSON.parse(event.data)
    if (!data) return
    if (data.action === 'ids') {
        const req = {
            AdminId: aid,
            Ids: data.data
        }
        httpTool.execute(urls.GetRecipeIngredients, req).then(res => {
            if (res === undefined) return
            initIngredients(res.data)
            updateSteps(ui.stepsContainer, getIngredients(ui.ingredientContainer, true))
            updateNutritionOverview()
        })
    }
})
