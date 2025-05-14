function initBody() {
    document.body.innerHTML = `
<div class="layui-fluid">
    <div class="layui-row">
        <form class="layui-form">
            <!-- 食谱图像上传 -->
            <div class="layui-form-item" id="m-upload"></div>

            <!-- 食材名称 -->
            <div class="layui-form-item">
                <label for="iName" class="layui-form-label">
                    <span class="x-red">*</span>食材名称</label>
                <div class="layui-input-inline">
                    <input type="text" id="iName" name="iName" required lay-verify="required" class="layui-input">
                </div>
            </div>

            <!-- 计量单位 -->
            <div class="layui-form-item">
                <label for="unit" class="layui-form-label">
                    <span class="x-red">*</span>计量单位</label>
                <div class="layui-input-inline">
                    <select name="unit" id="unit" lay-verify="required">
                        <option value="">请选择单位</option>
                        <option value="g">g</option>
                        <option value="ml">ml</option>
                    </select>
                </div>
            </div>

            <!-- 净含量 -->
            <div class="layui-form-item">
                <label for="content" class="layui-form-label">
                    <span class="x-red">*</span>净含量</label>
                <div class="layui-input-inline">
                    <input type="number" id="content" name="content" lay-verify="required|content" step="0.0001" min="0"
                           max="1" class="layui-input">
                </div>
                <div class="layui-form-mid">请输入0-1之间的小数（如：0.85）</div>
            </div>

            <!-- 单位换算部分 -->
            <div class="layui-form-item">
                <label class="layui-form-label">单位换算</label>
                <div class="layui-input-block">
                    <div id="unitContainer"></div>
                    <div style="margin-top: 10px">
                        <button type="button" class="layui-btn layui-btn-sm" id="addUnit">
                            <i class="layui-icon">&#xe654;</i>添加单位
                        </button>
                    </div>
                    <input type="hidden" name="quantity" id="quantity">
                </div>
            </div>

            <!-- 简介 -->
            <div class="layui-form-item">
                <label for="refer" class="layui-form-label">简介</label>
                <div class="layui-input-block">
                    <textarea id="refer" name="refer" class="layui-textarea" placeholder="请输入食材简介"></textarea>
                </div>
            </div>

            <!-- 过敏源 -->
            <div class="layui-form-item">
                <label for="allergy" class="layui-form-label">过敏源</label>
                <div class="layui-input-block">
                    <textarea id="allergy" name="allergy" class="layui-textarea"
                              placeholder="请输入过敏源信息"></textarea>
                </div>
            </div>

            <!-- 营养元素 -->
            <div class="layui-form-item">
                <label class="layui-form-label"><span class="x-red">*</span>营养元素</label>
                <div class="layui-input-block">
                    <div class="layui-form-mid layui-word-aux">请输入数值，支持小数点后4位（如：12.3456）</div>
                    <div id="nutrientContainer" class="nutrient-container"></div>
                </div>
            </div>

            <div class="layui-form-item">
                <label class="layui-form-label"></label>
                <button class="layui-btn" lay-filter="submit" lay-submit>提交</button>
            </div>
        </form>
    </div>
</div>
    `
}

const aid = sessionStorage.getItem('aid')
var releaseId = -1
var apiUrl = null
var ingredient = -1;
var nutrients = null;

initBody()
const ui = {
    addUnit: document.getElementById('addUnit'),
    unitContainer: document.getElementById('unitContainer'),
    imgUi: buildImg('m-upload', '食材图像', '点击上传，或将图片拖拽到此处'),
    iName: document.getElementById('iName'),
    unit: document.getElementById('unit'),
    content: document.getElementById('content'),
    refer: document.getElementById('refer'),
    allergy: document.getElementById('allergy'),
    quantity: document.getElementById('quantity'),
    nutrient: document.getElementById('nutrientContainer')
}

function addUnit(name, value) {
    const newItem = document.createElement('div');
    newItem.className = 'unit-item';
    newItem.innerHTML = `
                <div class="layui-input-inline">
                    <input type="number" placeholder="数值" class="layui-input unit-value" step="1" value="${value}">
                </div>
                <div class="unit-text">单位每</div>
                <div class="layui-input-inline">
                    <input type="text" placeholder="单位名称" class="layui-input unit-name" value="${name}">
                </div>
                <span class="del-unit"><i class="iconfont">&#xe630;</i>  删除</span>`;

    // 绑定删除事件
    newItem.querySelector('.del-unit').addEventListener('click', () => newItem.remove())
    return newItem
}

function renderNutrients() {
    ui.nutrient.innerHTML = nutrients.map(n => `
            <div class="nutrient-item">
                <label class="nutrient-label">${n.name} (${n.unit})</label>
                <div class="nutrient-input">
                    <input type="number"
                        class="layui-input"
                        data-id="${n.id}"
                        value="${n.value}"
                        step="0.0001"
                        min="0">
                </div>
            </div>`).join('');
}

ui.addUnit.addEventListener('click', () => {
    const newItem = addUnit('', '');
    ui.unitContainer.appendChild(newItem)
})

function ii(arrr, dddd) {
    initLay((form, upload, layer) => {
        // 添加自定义验证规则
        form.verify({
            content: value => {
                if (value < 0 || value > 1) {
                    return '净含量必须为0到1之间的小数'
                }
            }
        });
        myUpload(upload, layer, ui.imgUi, httpTool.fileUpload)
        form.render();

        // 表单提交处理
        form.on('submit(submit)', data => {
            const field = data.field
            // 处理单位换算
            const quantity = {};
            ui.unitContainer.querySelectorAll('.unit-item').forEach(item => {
                const name = item.querySelector('.unit-name').value;
                const value = item.querySelector('.unit-value').value;
                if (name && value) quantity[name] = Number(value);
            });

            // 收集营养元素数据
            const nutrients = {};
            document.querySelectorAll('.nutrient-input .layui-input').forEach(input => {
                const value = input.value.trim();
                if (value) {
                    const numValue = Number(value).toFixed(4);
                    nutrients[input.dataset.id] = parseFloat(numValue);
                }
            })


            const req = {
                AdminId: aid,
                ReleaseId: releaseId,
                Ingredient: {
                    IngredientId: ingredient?.ingredientId || -1,
                    fileUrl: ui.imgUi.fileUrl.value,
                    iName: field.iName,
                    unit: field.unit,
                    content: field.content,
                    quantity: quantity,
                    refer: field.refer,
                    allergy: field.allergy,
                    nutrients: nutrients
                }
            }

            sub(req)
            return false; // 阻止表单跳转
        })

        function initUi() {
            if (!ingredient) return
            ui.imgUi.init(ingredient.fileUrl)

            // 2. 基础信息
            ui.iName.value = ingredient.iName || ''
            ui.unit.value = ingredient.unit || 'g'
            ui.content.value = ingredient.content || 0
            ui.refer.value = ingredient.refer || ''
            ui.allergy.value = ingredient.allergy || ''

            Object.entries(ingredient.quantity).forEach(([key, value]) => {
                const newItem = addUnit(key, value)
                ui.unitContainer.appendChild(newItem)
            })

            form.render()
            form.render('select')
        }

        function sub(req) {
            httpTool.execute(apiUrl, req).then(res => {
                if (res === undefined) return
                layer.alert("添加成功", {icon: 6}, () => {
                    var index = parent.layer.getFrameIndex(window.name)
                    parent.layer.close(index)
                })
            })
        }

        renderNutrients()
        initUi()
    })
}