function initToast() {
    const co = document.createElement('div')
    co.id = 'toast-container'
    co.className = 'toast-container'
    document.body.appendChild(co)
}


function showToast(iconUnicode, title) {
    const co = document.getElementById('toast-container')
    if (co === undefined || co === null) return
    if (iconUnicode === undefined)
        iconUnicode = '&#xe621'
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.innerHTML = `
            <i class="icon iconfont">${iconUnicode}</i>
            <span>${title}</span>
        `;
    // 将新提示框插入到容器中
    co.appendChild(toast);

    // 触发显示动画
    setTimeout(() => {
        toast.classList.add('show')
    }, 10);

    // 2秒后移除提示框
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            co.removeChild(toast);
        }, 500); // 等待动画结束后移除
    }, 2000);
}

function getCheckData(table, pageId, name, w) {
    const checkStatus = table.checkStatus(pageId)
    var data = checkStatus.data;
    if (w) layer.alert(JSON.stringify(data, null, 4));
    const len = data.length;
    document.getElementById(name).innerText = len === 0 ? '无选中' : `选中了：${len} 个`;
}

function initLay(callback) {
    const params = callback.toString()
        .match(/\((.*?)\)/)[1]
        .split(',')
        .map(param => param.trim())

    // 加载模块
    layui.use(params, () => {
        const result = params.map(moduleName => layui[moduleName]);
        callback(...result);
    });
}


function closeLayer() {
    parent.layer.close(parent.layer.getFrameIndex(window.name));
}

function previewImg(obj) {
    layer.photos({
        photos: {"title": "Photos Demo", "start": 0, "data": [{"alt": obj.alt, "pid": obj.pid, "src": obj.src}]},
        shade: [0.5, '#000'], // 设置遮罩层颜色为黑色，透明度为0.5
        shadeClose: true, // 设置点击遮罩层关闭图片查看器（可选）
        success: () => {
            // 在图片预览成功后添加鼠标滚轮事件监听器（鼠标滑轮滚动缩放图片）
            $(document).on("mousewheel", ".layui-layer-photos img", function (event) {
                event.preventDefault(); // 阻止默认行为
                var delta = event.originalEvent.deltaY || event.originalEvent.detail || -event.originalEvent.wheelDelta; // 获取滚轮方向
                var scale = delta > 0 ? 0.9 : 1.1; // 定义缩放比例（缩小和放大）
                var img = $(this); // 获取当前图片元素
                var currentWidth = img.width(); // 当前宽度
                var currentHeight = img.height(); // 当前高度
                // 计算新的宽度和高度
                var newWidth = currentWidth * scale;
                var newHeight = currentHeight * scale;
                // 设置新的宽度和高度（注意：这里可能需要添加额外的逻辑来限制最大和最小缩放比例）
                img.css({
                    width: newWidth + 'px',
                    height: newHeight + 'px'
                });
                $('.layui-layer-page').css({
                    width: newWidth + 'px',
                    height: newHeight + 'px'
                })
                $('#layui-layer-photos').css({
                    width: newWidth + 'px',
                    height: newHeight + 'px'
                })
            });
        }
    })
}

function buildImg(id, title, tip = '点击上传图片') {
    return getImg(document.getElementById(id), title, tip = '点击上传图片')
}

var ans = 0;

function getImg(container, title, tip = '点击上传图片') {
    const pos = ans;
    ans += 1;
    container.innerHTML = `
        <label class="layui-form-label">${title}</label>
        <div class="layui-input-inline">
            <input type="hidden" id="fileUrl${pos}">
            <div class="layui-upload-list" style="margin-top:10px">
                <img id="mainPreview${pos}" style="max-width:200px; display:none; border-radius:8px;" onclick="previewImg(this)">
            </div>
        </div>
        <div class="layui-input-inline">
            <div class="layui-upload-drag" id="mainUpload${pos}" style="border-radius:10px;margin: 20px">
                <i class="layui-icon"></i>
                <p>${tip}</p>
            </div>
        </div>`
    var imgUi = {
        fileUrl: container.querySelector(`#fileUrl${pos}`),
        mainPreview: container.querySelector(`#mainPreview${pos}`),
        name: `#mainUpload${pos}`,
    }
    imgUi.init = (url, data = undefined) => {
        imgUi.fileUrl.value = data ? data : url
        imgUi.mainPreview.style.display = 'block'
        if (url) imgUi.mainPreview.src = url
    }
    return imgUi
}

function myUpload(upload, layer, imgUi, f) {
    upload.render({
        elem: imgUi.name,
        auto: false, // 关闭自动上传
        accept: 'images',
        choose: obj => {
            // 获取用户选择的文件
            const files = obj.pushFile();
            if (!files || typeof files !== 'object' || Object.keys(files).length === 0) {
                showToast(undefined, '没有文件或文件为空')
                return;
            }
            const fsarr = Object.values(files)
            if (fsarr.length === 0) {
                showToast(undefined, '没有文件或文件为空')
                return;
            }
            const file = fsarr.pop();

            // 手动触发上传逻辑
            layer.load(2); // 显示加载中
            f(file).then(res => {
                layer.closeAll('loading')
                if (res === undefined) {
                    return layer.msg('上传失败')
                }

                imgUi.init(res.url, res.outFileName)
            }).catch(err => {
                layer.closeAll('loading');
                layer.msg(`上传失败: ${err.message}`)
            });
        }
    });
}


function initTab(table, form, aid, pageId, isSearch, data, parameter) {
    var mCurr = parseInt(sessionStorage.getItem(`${aid}-${pageId}.curr`)) || 1
    var mLimit = parseInt(sessionStorage.getItem(`${aid}-${pageId}.limit`)) || 10
    var words;
    if (isSearch) {
        mCurr = 1
        words = parameter.getWords()
        sessionStorage.setItem(`${aid}-${pageId}.words`, JSON.stringify(words))
    } else {
        var json = sessionStorage.getItem(`${aid}-${pageId}.words`)
        if (json) {
            words = JSON.parse(json)
            if (words instanceof Object) {
                parameter.setWords(words)
            }
        } else {
            words = parameter.getWords()
        }
        sessionStorage.setItem(`${aid}-${pageId}.words`, JSON.stringify(words))
    }
    const arr = parameter.filter(data, words)

    table.init(pageId, {data: arr, page: {curr: mCurr, limit: mLimit}})
    table.reload(pageId, {
        done: (res, curr) => {
            const limit = layui.jquery(".layui-laypage-limits").find("option:selected").val();
            if (limit !== mLimit) {
                const oldStart = (mCurr - 1) * mLimit
                mCurr = Math.floor(oldStart / limit) + 1
                mLimit = limit
            } else {
                mCurr = curr
            }

            sessionStorage.setItem(`${aid}-${pageId}.curr`, mCurr)
            sessionStorage.setItem(`${aid}-${pageId}.limit`, mLimit)
        }
    })

    form.render()
}

function cmp(a, b) {
    return a === undefined || a === null || a === '' || ((b !== undefined && b !== null && b !== '') && (b.includes(a) || a.includes(b)))
}

function generateSearchBody(config) {
    const container = document.getElementById('search-body');
    if (!container) return;
    container.className = 'layui-card-body';
    const html = [
        `<form class="layui-form layui-col-space5">`
    ];

    Object.entries(config).forEach(([id, value]) => {
        if (typeof value === 'string') {
            html.push(`<div class="layui-inline layui-show-xs-block">
                           <input type="text" id="search-${id}"
                               placeholder="${value}"
                               autocomplete="off" class="layui-input">
                           </div>`);
        } else if (Array.isArray(value)) {
            html.push(`<div class="layui-input-inline layui-show-xs-block">
                           <select id="search-${id}">${value.map(opt => `<option value="${opt.value}">${opt.name}</option>`).join('')}
                           </select>
                       </div>`);
        }
    });

    html.push(`<div class="layui-inline layui-show-xs-block">
                   <button class="layui-btn" id="search-bnt" type="button">
                       <i class="iconfont20">&#xea13;</i>
                   </button>
               </div>
               <div class="layui-inline layui-show-xs-block">
                   <button class="layui-btn" id="search-close" type="button">
                       <i class="iconfont20">&#xe630;</i>
                   </button>
               </div></form>`);

    container.innerHTML = html.join('');
    const ui = {
        bnt: document.getElementById('search-bnt'),
        close: document.getElementById('search-close')
    };

    ui.close.addEventListener('click', () => {
        Object.keys(config).forEach(id => {
            const el = ui[id];
            if (el) {
                if (el.tagName === 'SELECT') {
                    el.value = el.options[0].value;
                } else if (el.tagName === 'INPUT') {
                    el.value = '';
                }
            }
        });
        ui.bnt.click();
    });

    Object.keys(config).forEach(id => {
        ui[id] = document.getElementById(`search-${id}`);
    });

    return {
        searchUi: ui,
        parameter: {
            getWords: () => {
                const words = {};
                Object.keys(config).forEach(id => {
                    words[id] = ui[id].value;
                });
                return words;
            },

            setWords: data => {
                Object.keys(config).forEach(id => {
                    const el = ui[id];
                    if (el && data[id] !== undefined) {
                        el.value = data[id];
                    }
                });
            },

            filter: (data, words) => {
                return data.filter(item =>
                    Object.keys(config).every(id => {
                        const value = words[id];
                        const itemValue = item[id];
                        const el = ui[id];
                        if (el && el.tagName === 'SELECT') {
                            return value === el.options[0].value.toString() || itemValue.toString() === value.toString();
                        }
                        if (el && el.tagName === 'INPUT') {
                            return cmp(value, itemValue);
                        }
                        return true;
                    })
                );
            }
        }
    };
}

function generateXNav(data) {
    const nav = document.querySelector('.x-nav')
    nav.innerHTML = `<span class="layui-breadcrumb">
                         <a href="${data[0].href}">${data[0].title}</a>
                         <a href="${data[1].href}">${data[1].title}</a>
                         <a href="${data[2].href}"><cite>${data[2].title}</cite></a>
                     </span>
                     <a class="layui-btn layui-btn-small" style="line-height:1.6em;margin-top:3px;float:right"
                         onclick="location.reload()" title="刷新">
                         <i class="layui-icon layui-icon-refresh" style="line-height:30px"></i>
                     </a>`
}

function generateTable(bar, data) {
    const container = document.getElementById('table-main')
    container.className = 'layui-card-body'
    container.innerHTML = `<table id="${bar.id}" lay-data="${bar.data}" lay-filter="${bar.id}" className="layui-table">
                               <thead>
                               <tr>${data.map(item => `<th lay-data="${item.data.trim()}">${item.name}</th>`).join('')}</tr>
                               </thead>
                           </table>`
}

function generateTab(cfg) {
    generateTabView()
    if (cfg.nav) generateXNav(cfg.nav)
    if (cfg.bar && cfg.data) generateTable(cfg.bar, cfg.data)
    return generateSearchBody(cfg.search)
}

function generateTabView() {
    document.body.innerHTML = `<div class="x-nav"></div>
                               <div class="layui-fluid">
                                   <div class="layui-row layui-col-space15">
                                       <div class="layui-col-md12">
                                           <div class="layui-card">
                                               <div id="search-body"></div>
                                               <div id="table-main"></div>
                                           </div>
                                       </div>
                                   </div>
                               </div>`
}

function generateImg(src, title) {
    return `<img src="${src}" alt="${title}" style="height:30px;" onClick="previewImg(this)"/>`
}


function generateStatus(status, statusMap, eve = null) {
    const {style, name} = statusMap[status.toString()] || statusMap[`${Unknown}`]
    return `<span class="layui-btn layui-btn-mini ${style}" ${eve === null ? '' : `lay-event="${eve}"`}>${name}</span>`
}

const presetReasons = {
    modify: [
        "内容不完整，请补充详细信息",
        "核心信息缺失，请完善关键内容",
        "描述过于简略，请扩充说明",
        "存在空白/未填写部分，请补全内容",
        "信息存在错误，请核实内容准确性",
        "时间信息矛盾，请统一时间节点",
        "图片/附件缺失，请重新上传",
        "图片分辨率过低，请替换高清图"
    ],
    reject: [
        "内容不完整，请补充详细信息",
        "核心信息缺失，请完善关键内容",
        "描述过于简略，请扩充说明",
        "存在空白/未填写部分，请补全内容",
        "信息存在错误，请核实内容准确性",
        "时间信息矛盾，请统一时间节点",
        "图片/附件缺失，请重新上传",
        "图片分辨率过低，请替换高清图",

        "包含敏感信息，请删除违规内容",
        "涉及第三方版权，请提供授权证明",
        "系统检测到异常内容，请人工复核",
        "提交版本错误，请重新上传正确版本"
    ]
};

function createDialog(type, callback) {
    const titleMap = {
        reject: '请选择或输入驳回原因',
        modify: '请选择或输入打回修改原因'
    };

    const formHtml = `
            <div style="margin: 15px 0;">
                <select id="presetSelect" style="width: 100%; margin-bottom: 10px; padding: 5px;">
                    <option value="">--- 选择常用原因 ---</option>
                    ${presetReasons[type].map(opt => `<option value="${opt}">${opt}</option>`).join('')}
                </select>
                <textarea id="reasonInput" 
                         style="width: 100%; height: 120px; padding: 5px;" 
                         placeholder="或手动输入${type === 'reject' ? '驳回' : '修改'}原因..."></textarea>
            </div>
        `;

    layer.open({
        type: 1,
        title: titleMap[type],
        area: ['500px', '400px'],
        content: formHtml,
        btn: ['确定', '取消'],
        success: (layerElem, index) => {
            $('#presetSelect', layerElem).on('change', function () {
                const selectedVal = $(this).val();
                if (selectedVal) {
                    $('#reasonInput', layerElem).val(selectedVal);
                }
            });
        },
        yes: (index, layerElem) => {
            var reason = $('#reasonInput', layerElem).val().trim();
            if (!reason) {
                layer.msg(`请填写${type === 'reject' ? '驳回' : '修改'}原因`);
                return;
            }
            callback(reason);
            layer.close(index);
        }
    });
}


function showRejectDialog(callback) {
    createDialog('reject', callback);
}

function showModifyDialog(callback) {
    createDialog('modify', callback);
}


const Unknown = 114514;
var myUtil = {}
myUtil.GetMap = function (data) {
    return data.reduce((map, info) => {
        map[info.value] = {style: info.style, name: info.name};
        return map;
    }, {})
}
myUtil.GetOptions = function (data, title) {
    return [
        {value: `${Unknown}`, name: title},
        ...data
            .filter(info => info.value !== Unknown) // 过滤掉 Unknown 项
            .map(info => ({value: `${info.value}`, name: info.name}))
    ]
}
const Status = Object.freeze({
    Default: 0, // 无意义
    // 审核相关状态（区间：100-199）
    Pending: 100, // 待审核
    Approve: 101, // 批准
    Reject: 102, // 驳回
    Locked: 103, // 审核已锁定 只读
    // 上下架相关状态（区间：200-299）
    On: 200, // 上架
    Off: 201, // 下架
    ForceOff: 202, // 强制下架
    ReportOff: 203, // 举报下架
    // 操作相关状态（区间：300-399）
    Deleted: 300, // 删除
    NeedEdit: 301, // 需修改
    Cancel: 302, // 取消
    Confirm: 303 // 待确认
});

const StatusInfo = [
    {value: Status.Pending, style: 'layui-btn-normal', name: '待审核'},
    {value: Status.Approve, style: 'layui-btn-success', name: '已批准'},
    {value: Status.Deleted, style: 'layui-btn-danger', name: '已删除'},
    {value: Status.NeedEdit, style: 'layui-btn-warm', name: '待修改'},
    {value: Status.Cancel, style: 'layui-btn-disabled', name: '已取消'},
    {value: Status.Confirm, style: 'layui-btn-warm', name: '待批准'},
    {value: Status.Reject, style: 'layui-btn-danger', name: '被驳回'},
    {value: Status.Locked, style: 'layui-btn-danger', name: '已锁定'},
    {value: Unknown, style: 'layui-btn-primary', name: '未知'}
];

const StatusMap = myUtil.GetMap(StatusInfo);
const StatusOptions = myUtil.GetOptions(StatusInfo, '审核状态');

const UserStatus = Object.freeze({
    Usable: 0,
    Logout: 1,
    Ban: 2
});
const UserStatusInfo = [
    {value: UserStatus.Usable, style: 'layui-btn-success', name: '可用'},
    {value: UserStatus.Logout, style: 'layui-btn-disabled', name: '已注销'},
    {value: UserStatus.Ban, style: 'layui-btn-danger', name: '已封禁'},
    {value: Unknown, style: 'layui-btn-warm', name: '?'}
];
const UserStatusMap = myUtil.GetMap(UserStatusInfo);
const UserStatusOptions = myUtil.GetOptions(UserStatusInfo, '使用状态');


const IdCategory = Object.freeze({
    Ingredient: 1,
    Recipe: 2,
    Collection: 3,
    Comment: 4,
    User: 5
});

const IdCategoryInfo = [
    {value: IdCategory.Ingredient, style: 'layui-btn layui-btn-normal', name: '食材'},
    {value: IdCategory.Recipe, style: 'layui-btn layui-btn-warm', name: '食谱'},
    {value: IdCategory.Collection, style: 'layui-btn layui-btn-danger', name: '合集'},
    {value: Unknown, style: 'layui-btn layui-btn-disabled', name: '未知'}
];

const IdCategoryMap = myUtil.GetMap(IdCategoryInfo);
const IdCategoryOptions = myUtil.GetOptions(IdCategoryInfo, '选择分类');

const AllIdCategoryInfo = [
    {value: IdCategory.Ingredient, style: 'layui-btn layui-btn-normal', name: '食材'},
    {value: IdCategory.Recipe, style: 'layui-btn layui-btn-warm', name: '食谱'},
    {value: IdCategory.Collection, style: 'layui-btn layui-btn-danger', name: '合集'},
    {value: IdCategory.Comment, style: 'layui-btn layui-btn-primary', name: '评论'},
    {value: Unknown, style: 'layui-btn layui-btn-disabled', name: '未知'}
];
const AllIdCategoryMap = myUtil.GetMap(AllIdCategoryInfo);
const AllIdCategoryOptions = myUtil.GetOptions(AllIdCategoryInfo, '选择分类');


const EntStatusInfo = [
    {value: Status.Pending, style: 'layui-btn-normal', name: '修改中'},
    {value: Status.On, style: 'layui-btn-success', name: '已上架'},
    {value: Status.Off, style: 'layui-btn-danger', name: '已下架'},
    {value: Status.ForceOff, style: 'layui-btn-danger', name: '已强制下架'},
    {value: Status.ReportOff, style: 'layui-btn-danger', name: '举报下架'},
    {value: Status.Deleted, style: 'layui-btn-danger', name: '已删除'},
    {value: Unknown, style: 'layui-btn-primary', name: '未知'}
];

const EntStatusMap = myUtil.GetMap(EntStatusInfo);
const EntStatusOptions = myUtil.GetOptions(EntStatusInfo, '选择状态');

const PublisherType = {
    User: true,
    Admin: false
};

const PublisherTypeInfo = [
    {value: PublisherType.User, name: '用户'},
    {value: PublisherType.Admin, name: '管理员'},
    {value: Unknown, name: '发布者类型'}
];

const PublisherTypeMap = myUtil.GetMap(PublisherTypeInfo);
const PublisherTypeOptions = myUtil.GetOptions(PublisherTypeInfo, '发布者类型');

const Gender = {
    Male: true,
    Female: false
};

const GenderInfo = [
    {value: Gender.Male, name: '男生'},
    {value: Gender.Female, name: '女生'},
    {value: Unknown, name: '性别'}
];

const GenderMap = myUtil.GetMap(GenderInfo);
const GenderOptions = myUtil.GetOptions(GenderInfo, '性别');


const Category = {
    Normal: 1,
    Emoji: 2
};

const CategoryInfo = [
    {value: Category.Normal, style: 'layui-btn-normal', name: '分类'},
    {value: Category.Emoji, style: 'layui-btn-warm', name: '表情'},
    {value: Unknown, style: 'layui-btn-warm', name: '?'}
];

const CategoryMap = myUtil.GetMap(CategoryInfo);
const CategoryOptions = myUtil.GetOptions(CategoryInfo, '选择分类');


const CommentStatus = Object.freeze({
    Usable: 0,
    ForceOff: 1
});

const CommentStatusInfo = [
    {value: CommentStatus.Usable, style: 'layui-btn-success', name: '可用'},
    {value: CommentStatus.ForceOff, style: 'layui-btn-danger', name: '已强制下架'},
    {value: Unknown, style: 'layui-btn-warm', name: '?'}
];
const CommentStatusMap = myUtil.GetMap(CommentStatusInfo);
const CommentStatusOptions = myUtil.GetOptions(CommentStatusInfo, '状态');


const ReportStatus = Object.freeze({
    Pending: 100,
    Reject: 102,
    Locked: 103
});
const ReportStatusInfo = [
    {value: ReportStatus.Pending, style: 'layui-btn-success', name: '处理中'},
    {value: ReportStatus.Reject, style: 'layui-btn-danger', name: '已驳回'},
    {value: ReportStatus.Locked, style: 'layui-btn-disabled', name: '处理完成'},
    {value: Unknown, style: 'layui-btn-warm', name: '?'}
];

const ReportStatusMap = myUtil.GetMap(ReportStatusInfo);
const ReportStatusOptions = myUtil.GetOptions(ReportStatusInfo, '举报状态');


const ReportTypes = Object.freeze({
    IllegalContent: 1, // 违法内容举报
    CopyrightInfringement: 2, // 侵权举报
    Harassment: 3, // 骚扰举报
    FalseInformation: 4, // 虚假信息举报
    Other: 5, // 其他类型举报
});
const ReportTypesInfo = [
    {value: ReportTypes.IllegalContent, style: 'layui-btn-success', name: '违法内容'},
    {value: ReportTypes.CopyrightInfringement, style: 'layui-btn-danger', name: '侵权'},
    {value: ReportTypes.Harassment, style: 'layui-btn-disabled', name: '骚扰'},
    {value: ReportTypes.FalseInformation, style: 'layui-btn-disabled', name: '虚假信息'},
    {value: ReportTypes.Other, style: 'layui-btn-disabled', name: '其他'},
    {value: Unknown, style: 'layui-btn-warm', name: '?'}
];

const ReportTypesMap = myUtil.GetMap(ReportTypesInfo);
const ReportTypesOptions = myUtil.GetOptions(ReportTypesInfo, '举报状态');