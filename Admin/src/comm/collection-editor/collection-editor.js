function getEditor(con) {
    con.innerHTML = `<div id="editor"></div>
<div class="c-info">
    <div class="layui-fluid">
        <div class="layui-row">
            <form class="layui-form">
                <!-- 图像上传 -->
                <div class="layui-form-item" id="c-upload"></div>
                <!-- 标题-->
                <div class="layui-form-item">
                    <label for="title" class="layui-form-label">
                        <span class="x-red">*</span>标题</label>
                    <input type="text" id="title" name="title" required lay-verify="required" class="layui-input">
                </div>
                <!-- 简介 -->
                <div class="layui-form-item">
                    <label for="summary" class="layui-form-label">简介</label>
                    <input type="text" id="summary" name="summary" class="layui-input" aria-multiline="true">
                </div>

                <div class="layui-form-item">
                    <label class="layui-form-label"></label>
                    <button class="layui-btn" lay-filter="submit" lay-submit>提交</button>
                </div>
            </form>
        </div>
    </div>
</div>
`
    return {
        imgUi: buildImg('c-upload', '*合集封面', '点击上传，或将图片拖拽到此处'),
        title: document.getElementById('title'),
        summary: document.getElementById('summary'),
        editor: document.getElementById('editor')
    }
}

function getMenus(con) {
    con.insertAdjacentHTML('beforebegin', `<div class="menus">
    <div id="toolbar" style="display: flex;justify-content: flex-end">
        <div id="image-btn" type="button" title="插入图片">
            <svg class="icon" viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="24">
                <path d="M821.8624 213.4016H278.8864c-67.4816 0-98.3552 30.72-98.4064 97.28q-0.3072 243.1488 0 486.4c0 67.3792 30.72 97.8432 98.3552 97.9456q271.6672 0.3072 543.2832 0c65.6384 0 97.28-31.744 97.28-96.6656V309.6576c-0.3584-65.024-31.6928-96.1024-97.536-96.256z m-496.9472 50.2272a83.6096 83.6096 0 1 1-83.5584 83.6096 83.6096 83.6096 0 0 1 83.5584-83.6096zM778.24 747.52H250.88a14.9504 14.9504 0 0 1-11.9808-23.9616c30.72-40.4992 91.4944-121.2928 116.7872-154.9312a15.0016 15.0016 0 0 1 23.5008-0.5632c20.48 24.4224 60.928 73.1136 81.1008 97.28a14.9504 14.9504 0 0 0 23.2448-0.3584c27.2896-34.8672 96.1024-122.88 123.7504-158.3104a14.9504 14.9504 0 0 1 23.7568 0c32.4608 42.9568 121.344 165.12 159.0272 217.0368A15.0016 15.0016 0 0 1 778.24 747.52z"
                      fill="#8CECFF"></path>
                <path d="M835.6352 711.2704c-46.08-61.44-132.5056-175.6672-171.3152-226.7136-10.24-13.7728-30.72-11.264-37.6832-1.7408-30.72 43.7248-105.0624 147.8144-135.5264 190.6688a15.7696 15.7696 0 0 1-26.0096 0.4608c-22.528-29.6448-45.2608-70.6048-68.0448-100.5056a15.7696 15.7696 0 0 0-26.2144 0.7168l-130.56 190.5152c-8.3456 12.1344-0.4608 29.4912 13.3632 29.4912h531.0976c75.776-0.1536 71.2192-58.9824 50.8928-82.8928z"
                      fill="#FFDE73"></path>
                <path d="M335.3088 357.9392m-95.6416 0a95.6416 95.6416 0 1 0 191.2832 0 95.6416 95.6416 0 1 0-191.2832 0Z"
                      fill="#FFDE73"></path>
                <path d="M811.6736 122.88H215.7568c-86.784 0-130.8672 43.8272-130.9696 129.8944v533.4528c0 86.4768 44.1344 130.3552 130.9184 130.4576h595.968c84.7872 0 129.6384-44.7488 129.6896-129.0752V251.4944C941.2096 167.3728 896.4096 122.88 811.6736 122.88z m83.5072 664.576c0 58.88-24.2688 82.8928-83.6608 82.9952H215.6544c-61.7984 0-84.7872-22.9376-84.8896-84.4288V252.672C130.8672 192.4096 154.6752 168.96 215.7056 168.96h595.8656c59.3408 0 83.5584 24.0128 83.6096 82.5856q0.1024 267.8784 0 535.9104z"
                      fill="#474747"></path>
                <path d="M326.4 439.7568A93.6448 93.6448 0 1 0 232.7552 346.112a93.7472 93.7472 0 0 0 93.6448 93.6448z m0-141.2096a47.5648 47.5648 0 1 1-47.5648 47.5648 47.616 47.616 0 0 1 47.5648-47.5648zM645.12 499.2512a37.5808 37.5808 0 0 0-59.5968-0.4096c-24.832 31.8976-83.9168 107.52-113.5616 145.2544L399.9232 558.08a37.0688 37.0688 0 0 0-29.7472-13.568 37.5296 37.5296 0 0 0-29.1328 14.9504c-24.5248 32.6144-83.6608 111.0016-113.3056 150.3232a37.5808 37.5808 0 0 0 30.0032 60.16h511.4368a37.5808 37.5808 0 0 0 30.4128-59.648c-35.9424-49.8176-122.9312-169.1648-154.4704-211.0464zM264.4992 737.28z m10.24-13.6704l96.6144-128 71.68 86.0672A37.5808 37.5808 0 0 0 501.76 680.96c24.4224-31.232 83.2512-106.3936 113.2544-144.896 32.512 43.4688 100.4032 136.704 137.5744 187.8016z"
                      fill="#474747"></path>
            </svg>
        </div>
        <div type="button" title="添加食材" onclick="getData('ingredient', [])">
            <svg class="icon" viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="24">
                <path d="M842.430253 205.970663c91.947878-2.717036 195.888606 149.404719 128.72461 237.244794-91.306915 103.493264-176.728263 97.930194-300.91373 220.954673l-79.443064-57.541497c48.281807-168.750498 110.185073-324.044816 251.632184-400.65797zM645.35244 98.700155C570.581549 34.680503 417.000496 0.342653 344.470959 114.361407c-80.188837 177.127353 8.183357 282.82569 76.806651 428.578133l81.208733 8.364761c11.509106-162.671433-9.404814-344.450803 142.866097-452.604146z"
                      fill="#BCE6B8"></path>
                <path d="M494.665795 312.06809c12.928092-121.190278 56.707037-212.864033 97.777008-252.539206 97.926163-94.604445 284.571204 17.112488 269.361447 142.092103-41.348125 180.819942-194.348684 280.326339-298.450661 410.490092l-58.694423-84.115236"
                      fill="#BCE6B8"></path>
                <path d="M620.072719 356.350936c-137.879489-67.123684-263.568598-90.807045-367.598012 122.891448-104.04554 213.710587-53.671535 452.85005 61.326803 508.827438 97.958412 47.697281 334.312309-51.845397 438.349786-265.551953 104.033446-213.702525 5.825099-299.031155-132.078577-366.166933z"
                      fill="#FF8787"></path>
                <path d="M233.330488 858.498658s46.874914 163.449456 200.060909 76.778433 152.178192-154.193797 106.827076-199.544915 6.046816-96.749049 64.499366-62.483761 98.764654 74.577392 98.764654 34.265289-50.39013-120.936312-108.84268-133.029943-46.358919-76.592997 32.249683-76.592997 116.905101 139.076758 124.967522 110.858285-18.539537-170.012267-148.779882-242.658709c0 0 222.671968 45.1294 193.123196 234.596289s-209.62294 348.699698-340.637277 386.996197-222.232566-54.792212-222.232567-129.184168z"
                      fill="#6E6E96" opacity=".15"></path>
                <path d="M758.589139 454.88178s73.049563-32.922895 110.17298-120.928249 146.80862 69.510161 37.123417 168.121629c0 0 165.549717-112.930328 38.219905-242.203182-19.494933-19.789212-45.850987-46.85879-80.716925-46.85879 0 0-41.29975 146.893275-138.528514 208.006425-15.262163 9.598312 33.729137 33.862167 33.729137 33.862167z"
                      fill="#6E6E96" opacity=".15"></path>
                <path d="M613.453472 341.854703s119.674543 41.89637 166.622019-72.400538c11.521199-28.057224 47.612626-87.420828 38.248124-128.373895-14.447858-43.077514-71.336299-116.485855-183.682101-108.435528 0 0 127.225-56.489351 215.790692 100.159453 9.219378 27.295326 24.650852 40.775693 6.401562 101.743719 0 0-63.241629 160.994449-123.750097 194.695368l-119.630199-87.388579zM496.588683 296.108528s-45.730051-148.937099-9.449157-185.217993-211.045957-31.032258-162.207844 222.325284c0 0-57.460873-225.40916 100.98182-284.135833 0 0 97.377918-37.429788 163.453488 14.379328-0.004031 0-92.778307 116.86882-92.778307 232.649214z"
                      fill="#6E6E96" opacity=".15"></path>
            </svg>
        </div>
        <div type="button" title="添加食谱" onclick="getData('recipe', [])">
            <svg class="icon" viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="24">
                <path d="M69.189189 465.643243v75.13946C69.189189 747.243243 227.632432 915.096216 421.362162 915.096216h180.998919c193.72973 0 352.311351-168.406486 352.311351-373.621621v-75.831352z"
                      fill="#FC6E5F"></path>
                <path d="M332.246486 845.907027h359.783784v110.702703h-359.783784z" fill="#FC6E5F"></path>
                <path d="M385.383784 367.256216L850.058378 98.940541a20.895135 20.895135 0 0 1 27.675676 7.61081 20.895135 20.895135 0 0 1-7.610811 28.367568L406.140541 403.234595a20.756757 20.756757 0 0 1-28.367568-7.610811 20.756757 20.756757 0 0 1 7.610811-28.367568zM519.887568 385.383784l371.545946-214.486487a20.618378 20.618378 0 0 1 27.675675 7.610811 20.756757 20.756757 0 0 1-7.610811 27.675676l-370.854054 215.178378a20.895135 20.895135 0 0 1-27.675675-7.610811 20.895135 20.895135 0 0 1 6.918919-28.367567z"
                      fill="#FB5341"></path>
                <path d="M69.327568 354.940541h885.621621v69.189189h-885.621621z" fill="#FC6E5F"></path>
                <path d="M221.405405 584.648649a179.061622 179.061622 0 0 0-137.271351 63.792432 364.627027 364.627027 0 0 0 251.156757 255.308108A179.891892 179.891892 0 0 0 221.405405 584.648649z"
                      fill="#FF8C80"></path>
            </svg>
        </div>
        <div type="button" title="添加合集" onclick="getData('collection', [])">
            <svg class="icon" viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg" width="24">
                <path d="M0 0h1024v1024H0V0z" fill="#202425" opacity=".01"></path>
                <path d="M682.666667 341.333333a136.533333 136.533333 0 0 1 273.066666 0v546.133334a68.266667 68.266667 0 0 1-68.266666 68.266666h-204.8V341.333333z"
                      fill="#FFAA44"></path>
                <path d="M68.266667 819.2a136.533333 136.533333 0 0 0 136.533333 136.533333h682.666667a68.266667 68.266667 0 0 1-68.266667-68.266666V204.8a136.533333 136.533333 0 0 0-136.533333-136.533333H204.8a136.533333 136.533333 0 0 0-136.533333 136.533333v614.4z"
                      fill="#FF7744"></path>
                <path d="M187.733333 273.066667A51.2 51.2 0 0 1 238.933333 221.866667h68.266667a51.2 51.2 0 1 1 0 102.4H238.933333A51.2 51.2 0 0 1 187.733333 273.066667z m0 204.8A51.2 51.2 0 0 1 238.933333 426.666667h409.6a51.2 51.2 0 0 1 0 102.4H238.933333A51.2 51.2 0 0 1 187.733333 477.866667zM238.933333 631.466667a51.2 51.2 0 0 0 0 102.4h204.8a51.2 51.2 0 0 0 0-102.4H238.933333z"
                      fill="#FFFFFF"></path>
            </svg>
        </div>
    </div>
    <textarea readonly id="html-info"></textarea>
</div>`)
}

function getHtml(html) {
    const container = document.createElement('div');
    container.innerHTML = html;
    const images = [];
    const ids = [];
    const tags = [];
    const mp = {};

    // 处理图片
    const imgs = container.querySelectorAll('img.insert-img');
    imgs.forEach((img, index) => {
        const dataFlag = img.getAttribute('data-flag') || '';
        images.push(dataFlag);
        img.removeAttribute('data-flag');
        img.setAttribute('data-flag', index.toString());
        ['src', 'alt', 'width', 'contenteditable'].forEach(attr => img.removeAttribute(attr));
    });

    // 处理标签
    const tgs = container.querySelectorAll('div.tag');
    tgs.forEach(tg => {
        tg.removeAttribute('contenteditable');
        const dataFlag = tg.getAttribute('data-flag') || '';
        if (dataFlag.includes('_')) {
            const [tag, id] = dataFlag.split('_').map(Number);
            tags.push(tag);
            ids.push(id);
            tg.innerHTML = '';
        }
    });

    // 处理链接
    const ass = container.querySelectorAll('a');
    ass.forEach(a => a.removeAttribute('contenteditable'));

    // 构建映射关系
    ids.forEach((id, i) => {
        const idc = tags[i];
        if (!mp[idc]) mp[idc] = new Set();
        mp[idc].add(id);
    });

    // 转换 Set 为数组
    Object.keys(mp).forEach(k => mp[k] = [...mp[k]]);

    return {
        html: container.innerHTML, images, mp
    };
}

function setHtml(html, images, tabs) {
    if (!html || !tabs || !images) return null;
    const container = document.createElement('div');
    container.innerHTML = html;

    // 还原图片
    const imgs = container.querySelectorAll('img.insert-img');
    imgs.forEach(tg => {
        const dataFlag = parseInt(tg.getAttribute('data-flag'));
        const imgData = images[dataFlag];
        tg.setAttribute('data-flag', imgData.id);
        tg.setAttribute('src', imgData.url);
        tg.setAttribute('alt', '');
        tg.setAttribute('width', '400');
        tg.setAttribute('contenteditable', 'false');
    });

    // 还原标签
    const tbs = tabs.reduce((map, t) => {
        map.set(`${t.idCategory}_${t.id}`, t);
        return map;
    }, new Map());

    container.querySelectorAll('div.tag').forEach(tg => {
        const dataFlag = tg.getAttribute('data-flag');
        const tabData = tbs.get(dataFlag);
        if (tabData) {
            tg.innerHTML = `
                <img src="${tabData.fileUrl}">
                <div class="tag-content">
                    <div class="tag-title">${tabData.title}</div>
                    <div class="tag-refer">${tabData.refer}</div>
                </div>`;
        }
    });
    return container.innerHTML;
}

const ImageBlot = Quill.import('formats/image');

class CustomImageBlot extends ImageBlot {
    static create(value) {
        const node = super.create(value.src);
        node.setAttribute('class', value.class);
        node.setAttribute('data-flag', value.flag);
        node.setAttribute('width', value.width);
        node.setAttribute('contenteditable', 'false');
        return node;
    }

    static value(node) {
        return {
            src: node.src,
            class: node.getAttribute('class'),
            flag: node.getAttribute('data-flag'),
            width: node.getAttribute('width')
        };
    }

}

CustomImageBlot.blotName = 'customImage';
CustomImageBlot.tagName = 'img';
Quill.register(CustomImageBlot);
const BlockEmbed = Quill.import('blots/block/embed');

class CustomCardBlot extends BlockEmbed {
    static create(value) {
        // 直接创建带 class="tag" 的 div（核心改动）
        const node = super.create();
        node.classList.add('tag');      // 添加 class
        node.setAttribute('data-flag', value.flag);
        node.innerHTML = `
            <img src="${value.src ?? ''}">
            <div class="tag-content">
                <div class="tag-title">${value.title ?? ''}</div>
                <div class="tag-refer">${value.refer ?? ''}</div>
            </div>
        `;
        node.setAttribute('contenteditable', 'false');
        return node;
    }

    static value(node) {
        // 直接从当前节点提取属性（不再查询子级 .tag）
        return {
            flag: node.getAttribute('data-flag') || '',
            src: node.querySelector('img')?.src || '',
            title: node.querySelector('.tag-title')?.textContent || '',
            refer: node.querySelector('.tag-refer')?.textContent || ''
        };
    }
}

CustomCardBlot.blotName = 'customCard';
CustomCardBlot.tagName = 'div';  // 直接使用 div 标签
CustomCardBlot.className = 'tag'; // 关键标识
Quill.register(CustomCardBlot);

function createQuill() {
    return new Quill('#editor', {
        theme: 'snow', modules: {
            toolbar: [[{'header': [1, 2, 3, 4, 5, 6, false]}],  // 多级标题
                ['bold', 'italic', 'underline', 'strike'], // 基础格式
                [{'color': []}, {'background': []}],   // 颜色选择器
                [{'font': []}, {'size': ['small', false, 'large', 'huge']}], // 字体与字号
                [{'align': []}, {'direction': 'rtl'}], // 对齐与文本方向
                ['blockquote', 'code-block'],              // 引用块与代码块
                [{'list': 'ordered'}, {'list': 'bullet'}, {'list': 'check'}], // 多种列表
                [{'script': 'sub'}, {'script': 'super'}], // 上下标
                [{'indent': '-1'}, {'indent': '+1'}],   // 缩进控制
                ['clean'],                                 // 清除格式
            ], history: {delay: 2000}     // 增强历史记录功能
        }
    })
}

function getData(flag, ids) {
    sessionStorage.setItem('select-flag', flag)
    sessionStorage.setItem('ids', ids)
    xadmin.open('搜索食材', '../../comm/list-select/list-select.html', '', '', true)
}

var releaseId = -1
var ce = JSON.parse(sessionStorage.getItem('collection-edit'))
var cef = ce.flag || 'add'
var ceid = ce.id || -1

function initData(collection) {
    ui.imgUi.init(collection.fileUrl)
    ui.summary.value = collection.summary
    ui.title.value = collection.title
    quill.pasteHTML(setHtml(collection.html, collection.images, collection.tabs))
}

function init() {
    window.addEventListener('message', (event) => {
        const flag = sessionStorage.getItem('select-flag')
        const data = JSON.parse(event.data)
        if (data && data.action === 'ids' && data.data.length > 0) {
            const f = flag === 'ingredient' ? IdCategory.Ingredient : flag === 'recipe' ? IdCategory.Recipe : flag === 'collection' ? IdCategory.Collection : -1
            const req = {
                AdminId: aid, Flag: f, Ids: data.data
            }
            httpTool.execute(urls.GetTabs, req).then(res => {
                if (res === undefined) return
                if (res.data.length == 0) return
                res.data.forEach(async r => {
                    quill.focus();
                    const range = quill.getSelection();
                    if (!range) {
                        layer.msg('此处不许插入')
                        return;
                    }
                    await quill.insertEmbed(range.index, 'customCard', {
                        flag: `${r.idCategory}_${r.id}`, src: r.fileUrl, title: r.title, refer: r.refer
                    });
                    quill.setSelection(range.index + 1, Quill.sources.SILENT);
                })
            })
        }
    })

    document.querySelector('#toolbar').addEventListener('mousedown', e => {
        e.preventDefault();
        quill.focus();
    });

    //图片上传插入
    initLay((form, layer, upload) => {
        myUpload(upload, layer, ui.imgUi, httpTool.fileUpload)
        upload.render({
            elem: '#image-btn', auto: false, // 关闭自动上传
            accept: 'images', choose: obj => {
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

                httpTool.fileUpload(file).then(res => {
                    if (res === undefined) {
                        return layer.msg('上传失败')
                    }
                    quill.focus();
                    const range = quill.getSelection();
                    if (!range) {
                        layer.msg('此处不许插入')
                        return;
                    }
                    quill.insertEmbed(range.index, 'customImage', {
                        src: res.url, class: 'insert-img', flag: res.outFileName, width: '300'
                    });
                    quill.setSelection(range.index + 2, Quill.sources.SILENT);
                }).catch(err => {
                    layer.msg(`上传失败: ${err.message}`)
                });
            }
        });

        // 表单提交处理
        form.on('submit(submit)', data => {
            const htmls = getHtml(quill.root.innerHTML)
            const req = {
                AdminId: aid,
                ReleaseId: releaseId,
                Collection: {
                    CollectionId: ceid,
                    FileUrl: ui.imgUi.fileUrl.value,
                    Summary: ui.summary.value,
                    Title: ui.title.value,
                    Content: {
                        Dirs: htmls.mp, Images: htmls.images, Html: htmls.html
                    }
                }
            }
            if (cef === 'add') {
                httpTool.execute(urls.AddCollection, req).then(res => {
                    if (res === undefined) return
                    layer.alert("添加成功", {icon: 6}, () => {
                        var index = parent.layer.getFrameIndex(window.name)
                        parent.layer.close(index)
                    })
                })
            } else if (cef === 'edit') {
                httpTool.execute(urls.EditCollection, req).then(res => {
                    if (res === undefined) return
                    layer.alert("修改成功", {icon: 6}, () => {
                        var index = parent.layer.getFrameIndex(window.name)
                        parent.layer.close(index)
                    })
                })
            }

            return false;
        })
    })
}