const jsonUi = {
    lTitle: document.getElementById('lTitle'),
    rTitle: document.getElementById('rTitle'),
    l: document.getElementById('lJson'),
    r: document.getElementById('rJson'),
    container: document.querySelector('.list-container'),
    content: document.getElementById('list-content'),
    icon: document.querySelector('.toggle-btn .iconfont')
}

const allSubLists = document.querySelectorAll('.sub-list');
const map = new Map();
const viewer = new JsonDiffViewer({
    mode: {
        '*': 'strict',
        steps: 'strict',//严格
        ingredients: 'loose',//宽松
        categories: 'loose',//宽松
    },
    onLeftUpdate: html => {
        jsonUi.l.innerHTML = html;
        initJsonImg(jsonUi.l)
    },
    onRightUpdate: html => {
        jsonUi.r.innerHTML = html;
        initJsonImg(jsonUi.r)
    }
})

var now = null
var list = []

function rStatus(status) {
    return generateStatus(status, StatusMap);
}

function idCategory(status) {
    return generateStatus(status, idCategoryMap)
}

function author(isUser, name, fileUrl) {
    return `<img src="${fileUrl}" alt="" style="height:38px;" onClick="fileFun(this)"/>${getNameSpan(isUser, name)}`
}

function fileFun(ui) {
    previewImg(this)
    event.stopPropagation()
}

function getNameSpan(isUser, name) {
    return `<span class="layui-btn layui-btn-mini ${isUser ? 'layui-btn-normal' : 'layui-btn-success'}">${isUser ? '用户' : '管理员'} ${name} </span>`
}


function createListItem(data) {
    return `<div class="list-item" data-flag="${data.id}" onClick="toggleSubList(this)">
                <div class="title-wrapper">
                    <span class="title-text">${data.title}</span>
                    <br/>
                    <br/>
                    <span class="date-badge">${data.createDate}</span>
                    <span class="date-badge" onclick="getInfo(this)">查看参考</span>
                </div>
                <div class="sub-list" onclick="event.stopPropagation()">${data.releaseFlowHistories.map((item, index, arr) => `
                    <div class="sub-list-item">
                        <div class="content-container ${index === arr.length - 1 ? 'last-item' : ''}">
                            <div class="item-header">
                                <span class="date-tag">${item.createDate}</span>
                            </div>
                            <div class="item-header2">
                                ${author(item.user.isUser, item.user.name, item.user.fileUrl)}
                            </div>
                            <div class="item-content">
                                ${rStatus(item.status)}<span class="info-text">${item.info}</span>
                            </div>
                        </div>
                    </div>`).join('')}
                </div>
            </div>`
}

function getInfo(ui) {
    event.stopPropagation()
    const data = parseInt(ui.parentElement.parentElement.getAttribute('data-flag'));
    const l = list.find(ll => ll.id.toString() === data.toString())
    layer.alert(l.releaseInfo);
}


function initJsonImg(ui) {
    ui.querySelectorAll('img').forEach(img => img.addEventListener('click', () => previewImg(img)))
}

function toggleList() {
    jsonUi.icon.classList.toggle('rotated');
    jsonUi.container.classList.toggle('collapsed');
    allSubLists.forEach(subList => subList.classList.remove('active'));
}

toggleFun = null

function toggleSubList(item) {
    const sub = item.querySelector('.sub-list')
    const data = parseInt(item.getAttribute('data-flag'));
    allSubLists.forEach(subList => {
        if (subList !== sub) {
            subList.classList.remove('active');
        }
    });

    if (sub.classList.contains('active')) {
        sub.style.height = sub.scrollHeight + 'px';
        sub.style.height = '0';
    } else {
        sub.style.height = sub.scrollHeight + 'px';
    }
    item.classList.toggle('active');
    sub.classList.toggle('active');

    const l = list.find(ll => ll.id.toString() === data.toString())
    viewer.setRJson(JSON.parse(l.content));
    toggleFun?.call(l, l)
}


function syncScroll(scrollFrom, scrollTo) {
    scrollTo.scrollTop = scrollFrom.scrollTop;
    scrollTo.scrollLeft = scrollFrom.scrollLeft;
}

function initReleaseEditor(createDate, now, list) {
    this.now = now
    this.list = list
    if (createDate) {
        jsonUi.lTitle.innerHTML = `数据库记录 ${createDate}`
        viewer.setLJson(now)
    } else {
        jsonUi.lTitle.innerHTML = '数据库记录 暂无'
    }

    if (list.length > 0) {
        jsonUi.content.innerHTML = list.map(l => createListItem(l))
    }

    jsonUi.l.addEventListener('scroll', () => syncScroll(jsonUi.l, jsonUi.r));
    jsonUi.r.addEventListener('scroll', () => syncScroll(jsonUi.r, jsonUi.l));
    toggleSubList(document.querySelector(`.list-item[data-flag="${id}"]`))
}