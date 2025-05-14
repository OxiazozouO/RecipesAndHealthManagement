function createStep(stepsContainer, stepData) {
    if (!stepData) {
        stepData = {
            id: '',
            title: '',
            fileUrl: '',
            refer: '',
            requiredTime: '',
            requiredIngredient: '||',
            summary: ''
        }
    }
    const card = document.createElement('div');
    card.className = 'step-card';
    const ddd = getTime(stepData.requiredTime)

    // 添加折叠状态控制
    card.innerHTML = `
        <div class="step-header">
            <h3>
                <span class="toggle-step"><i class="iconfont">&#xe65a;</i></span>
                <span class="step-title-text">${stepData.title || '新步骤'}</span>
            </h3>
            <div class="step-actions">
                <span class="move-step up" title="上移"><i class="iconfont20">&#xe611;</i></span>
                <span class="move-step down" title="下移"><i class="iconfont20">&#xe614;</i></span>
                <span class="del-step"><i class="iconfont18">&#xe630;</i> 删除</span>
            </div>
        </div>
        <div class="step-content">
            <div class="layui-form-item">
                <label class="layui-form-label"><span class="x-red">*</span>步骤标题</label>
                <div class="layui-input-block">
                    <input type="text" id="stepTitle" required lay-verify="required"
                           value="${stepData.title || ''}" class="layui-input" placeholder="例如：步骤1">
                </div>
            </div>
            <div class="layui-form-item" id="step-upLoad"></div>
            <div class="layui-form-item">
            <label class="layui-form-label">所需时间</label>
            <div class="layui-input-inline" style="width: 280px;">
                <div class="layui-row layui-col-space5">
                    <div class="layui-col-md4">
                        <input type="number" 
                             id="hours"
                             value="${ddd.hours}"
                             placeholder="小时" 
                             autocomplete="off"
                             class="layui-input"
                             min="0" 
                             max="1000">
                    </div>
                    <div class="layui-col-md4">
                        <input type="number" 
                             id="minutes" 
                             value="${ddd.minutes}"
                             placeholder="分钟" 
                             autocomplete="off"
                             class="layui-input"
                             min="0" 
                             max="59">
                    </div>
                    <div class="layui-col-md4">
                        <input type="number" 
                             id="seconds" 
                             value="${ddd.seconds}"
                             placeholder="秒数" 
                             autocomplete="off"
                             class="layui-input"
                             min="0" 
                             max="59">
                    </div>
                </div>
            </div>
            <input type="hidden" id="stepId" value="${stepData.id}">
            <input type="hidden" id="requiredTime" value="${stepData.requiredTime}">
            <input type="hidden" id="requiredIngredient" value="${stepData.requiredIngredient}">
            </div>
            <div class="layui-form-item layui-input-inline">
                <div class="layui-form">
                    <table class="layui-table" lay-size="sm">
                        <thead>
                            <tr>
                                <th>食谱ID</th>
                                <th>食材名称</th>
                                <th>模式</th>
                                <th>时间比例</th>
                                <th>实际时间</th>
                            </tr>
                        </thead>
                        <tbody class="ttbody">
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label"><span class="x-red">*</span>步骤说明</label>
                <div class="layui-input-block">
                    <textarea id="refer" class="layui-textarea" required lay-verify="required"
                              placeholder="请输入步骤说明">${stepData.refer || ''}</textarea>
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">秘籍、技巧</label>
                <div class="layui-input-block">
                    <textarea id="summary" class="layui-textarea"
                              placeholder="请输入步骤说明">${stepData.summary || ''}</textarea>
                </div>
            </div>
        </div>`
    stepsContainer.appendChild(card)

    const ui = {
        id: card.querySelector('#stepId'),
        title: card.querySelector('#stepTitle'),
        refer: card.querySelector('#refer'),
        summary: card.querySelector('#summary'),
        hours: card.querySelector('#hours'),
        minutes: card.querySelector('#minutes'),
        seconds: card.querySelector('#seconds'),
        requiredTime: card.querySelector('#requiredTime'),
        ratios: card.querySelectorAll('.ratio-input'),
        tbody: card.querySelector('.ttbody'),
        requiredIngredient: card.querySelector('#requiredIngredient'),
        imgUi: getImg(card.querySelector('#step-upLoad'), '步骤图片', '点击上传步骤图'),
    }
    ui.imgUi.init(stepData.fileUrl)


    stepsContainer.uis.push(ui)
    setUi(ui)

    function setUi(ui) {
        if (stepsContainer.addUi) {
            stepsContainer.addUi(ui)
        } else {
            setTimeout(() => setUi(ui), 100)
        }
    }


    // 定义高亮计时器变量
    let highlightTimer;

    card.querySelector('.move-step.up').addEventListener('click', () => {
        const prev = card.previousElementSibling
        if (prev) {
            // 添加动画类
            card.classList.add('step-card-moving-up')
            prev.classList.add('step-card-target')

            // 等待动画完成后再移动
            setTimeout(() => {
                stepsContainer.insertBefore(card, prev)
                // 移除动画类
                card.classList.remove('step-card-moving-up')
                prev.classList.remove('step-card-target')

                // 添加高亮边框效果
                card.classList.add('step-card-highlight')

                // 清除之前的计时器
                clearTimeout(highlightTimer)

                // 延迟移除高亮效果
                highlightTimer = setTimeout(() => {
                    card.classList.remove('step-card-highlight')
                }, 3000) // 保持高亮3秒
            }, 300)
        }
    })

    card.querySelector('.move-step.down').addEventListener('click', () => {
        const next = card.nextElementSibling
        if (next) {
            // 添加动画类
            card.classList.add('step-card-moving-down')
            next.classList.add('step-card-target')

            // 等待动画完成后再移动
            setTimeout(() => {
                stepsContainer.insertBefore(card, next.nextSibling)
                // 移除动画类
                card.classList.remove('step-card-moving-down')
                next.classList.remove('step-card-target')

                // 添加高亮边框效果
                card.classList.add('step-card-highlight')

                // 清除之前的计时器
                clearTimeout(highlightTimer)

                // 延迟移除高亮效果
                highlightTimer = setTimeout(() => {
                    card.classList.remove('step-card-highlight')
                }, 3000) // 保持高亮3秒
            }, 300)
        }
    })

    // 绑定标题同步
    const titleDisplay = card.querySelector('.step-title-text');
    ui.title.addEventListener('input', (e) => {
        titleDisplay.textContent = e.target.value || '新步骤';
    });

    // 绑定折叠切换
    card.querySelector('.toggle-step').addEventListener('click', function () {
        card.classList.toggle('collapsed');
        this.innerHTML = card.classList.contains('collapsed') ? '<i class="iconfont">&#xe659;</i>' : '<i class="iconfont">&#xe65a;</i>';
    })

    // 绑定删除事件
    card.querySelector('.del-step').addEventListener('click', () => {
        if (confirm('确定要删除该步骤吗？')) card.remove()
        stepsContainer.uis.remove(ui)
    })
}

function convert(requiredIngredient) {
    const result = []
    if (requiredIngredient === null) return result
    const [ls, rs, ratiosStr] = requiredIngredient.split('|')

    // 处理食材分段
    const l = ls ? ls.split('.') : []
    let r = rs ? rs.split('.') : []
    const ratios = ratiosStr ? ratiosStr.split('.').map(Number) : []

    r = [...l, ...r]

    for (let i = 0; i < r.length; i++) {
        result.push({
            flag: l.includes(r[i]) ? -1 : 1,
            id: r[i],
            ratio: ratios[i]
        })
    }
    return result
}

function convertTo(result) {
    const l = []
    const r = []

    for (let i = 0; i < result.length; i++) {
        const w = result[i]
        if (!w) continue
        if (w.flag === -1) {
            l.push({id: w.id, ratio: w.ratio})
        } else if (w.flag === 1) {
            r.push({id: w.id, ratio: w.ratio})
        }
    }
    l.sort((id, ratio) => id)
    r.sort((id, ratio) => id)
    const arr = [...l, ...r].map(r => r.ratio)
    if (l.length > 0) {
        return `${l.map(item => item.id).join('.')}|${r.map(item => item.id).join('.')}|${arr.join('.')}`
    } else {
        return `|${r.map(item => item.id).join('.')}|${arr.join('.')}`
    }
}

function updateSteps(stepsContainer, ingredients) {
    stepsContainer.uis.forEach(ui => {
        const ca = convert(ui.requiredIngredient.value)
        var result = ingredients.map(i => {
            const w = ca.filter(x => x.id.toString() === i.ingredientId.toString())
            return w.length > 0 ? {
                id: w[0].id,
                flag: w[0].flag,
                name: i.iName,
                ratio: w[0].ratio
            } : {
                id: i.ingredientId,
                flag: 0,
                name: i.iName,
                ratio: ''
            }
        })

        ui.tbody.innerHTML = ''
        for (let j = 0; j < result.length; j++) {
            const tr = document.createElement('tr');
            const i = j
            tr.innerHTML = `
                <td id="iid">${result[i].id}</td>
                <td id="name">${result[i].name}</td>
                <td>
                    <div class="layui-input-inline layui-show-xs-block">
                        <select class="flag-select">
                            <option value="0">无</option>
                            <option value="-1">占位</option>
                            <option value="1">外显</option>
                    </select>
                    </div>
                </td>
                <td><input id="ratio" type="number" class="layui-input dosage-input" value="${result[i].ratio}" min="0" step="1"></td>
                <td id="time"></td>`
            ui.tbody.appendChild(tr)

            const trui = getUi(tr)
            trui.ratio.addEventListener('input', () => {
                result[i].ratio = parseFloat(trui.ratio.value)
                updateAllTime(ui)
            })
            trui.flag.value = result[i].flag
        }
        updateAllTime(ui)
        layui.form.render('select')

        ui.hours.addEventListener('input', () => handleTimeChange(ui))
        ui.minutes.addEventListener('input', () => handleTimeChange(ui))
        ui.seconds.addEventListener('input', () => handleTimeChange(ui))

        handleTimeChange(ui)
        ui.requiredTime.addEventListener('input', () => updateAllTime(ui))
        ui.ratios.forEach(input => input.addEventListener('input', () => updateAllTime(ui)))

    })
}

function handleTimeChange(ui) {
    // 获取当前值，空值视为0
    let h = parseInt(ui.hours.value) || 0
    let m = parseInt(ui.minutes.value) || 0
    let s = parseInt(ui.seconds.value) || 0

    // 处理秒进位到分钟
    m += Math.floor(s / 60)
    s = s % 60

    // 处理分钟进位到小时
    h += Math.floor(m / 60)
    m = m % 60

    // 更新输入框的值，0则设为空
    updateInputValue(ui.hours, h)
    updateInputValue(ui.minutes, m)
    updateInputValue(ui.seconds, s)

    // 生成格式化时间字符串
    const formattedTime =
        `${String(h).padStart(4, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
    ui.requiredTime.value = formattedTime;

    updateAllTime(ui)
}

function updateInputValue(input, value) {
    const newValue = value ? value.toString() : '';
    if (input.value !== newValue) {
        input.value = newValue;
    }
}

function getUi(row) {
    return {
        flag: row.querySelector('.flag-select'),
        ratio: row.querySelector('#ratio'),
        time: row.querySelector('#time'),
        name: row.querySelector('#name'),
        iid: row.querySelector('#iid'),
    }
}

function updateAllTime(ui) {
    const time = splitTime(ui.requiredTime.value)
    var total = 0;

    const rows = Array.from(ui.tbody.rows).map(row => getUi(row));

    rows.forEach(row => {
        if (row.flag.value !== "0") {
            const aa = row.ratio.value
            total += parseInt(aa ? aa : 0)
        }
    })
    rows.forEach(row => {
        if (row.flag.value !== "0") {
            const aa = row.ratio.value
            if (!aa) {
                row.time.innerText = ''
            } else {
                const bb = total === 0 ? 0 : parseInt(aa ? aa : 0) * time / total
                row.time.innerText = formatTime(bb)
            }
        } else {
            row.time.innerText = ''
        }
    })

    var list = rows.map(row => {
        if (row.flag.value && row.flag.value !== "0" && row.ratio.value) {
            return {
                flag: parseInt(row.flag.value),
                ratio: parseInt(row.ratio.value),
                id: row.iid.innerText
            }
        }
    })

    ui.requiredIngredient.value = convertTo(list)
}

function splitTime(timeStr) {
    if (timeStr === null) return 0
    // 解析时间（兼容带/不带秒的格式）
    const parts = timeStr.split(':');
    const hours = parseInt(parts[0], 10) || 0;
    const minutes = parseInt(parts[1], 10) || 0;
    const seconds = parts[2] ? parseInt(parts[2], 10) : 0; // 无秒时默认为0
    return hours * 3600 + minutes * 60 + seconds
}

function getTime(timeStr) {
    if (timeStr === null) return 0
    // 解析时间（兼容带/不带秒的格式）
    const parts = timeStr.split(':');
    const hours = parseInt(parts[0], 10) || '';
    const minutes = parseInt(parts[1], 10) || '';
    const seconds = parts[2] ? parseInt(parts[2], 10) : ''; // 无秒时默认为0
    return {
        hours: hours === 0 ? '' : hours,
        minutes: minutes === 0 ? '' : minutes,
        seconds: seconds === 0 ? '' : seconds
    }
}

function formatTime(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60)
    const secs = parseInt(seconds % 60);

    const formattedHours = hours.toString().padStart(2, '0')
    const formattedMinutes = minutes.toString().padStart(2, '0')
    const formattedSeconds = secs.toString().padStart(2, '0')

    return `${formattedHours}:${formattedMinutes}:${formattedSeconds}`
}

function parseTimeInput(timeStr) {
    if (!timeStr) return null;
    const [h, m, s] = timeStr.split(':');
    return `${h.padStart(2, '0')}:${m.padStart(2, '0')}:${s.padStart(2, '0')}`
}

function getSteps(stepsContainer) {
    const map = new Map()
    stepsContainer.uis.forEach(step => {
        const id = !step.id.value ? -1 : step.id.value
        map[id.toString()] = step
    })
    const steps = Array.from(stepsContainer.querySelectorAll('#stepId')).map(id => {
        const s = map[id.value]
        updateAllTime(s)
        var time = parseTimeInput(s.requiredTime.value)
        time = time === '0000:00:00' ? null : time
        return {
            id: !s.id.value ? -1 : s.id.value,
            title: s.title.value,
            fileUrl: s.imgUi.fileUrl.value,
            summary: s.summary.value === "" ? null : s.summary.value,
            requiredTime: time,
            requiredIngredient: s.requiredIngredient.value,
            refer: s.refer.value
        }
    })
    return steps
}