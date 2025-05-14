let RE = {
    currentSelection: {
        "startContainer": 0,
        "startOffset": 0,
        "endContainer": 0,
        "endOffset": 0
    },
    editor: document.getElementById('editor'),
}

RE.setHtml = contents => RE.editor.innerHTML = decodeURIComponent(contents.replace(/\+/g, '%20'))
RE.getHtml = () => RE.editor.innerHTML
RE.getText = () => RE.editor.innerText

document.addEventListener("selectionchange", () => RE.backuprange())

RE.callback = () => window.location.href = "re-callback://" + encodeURIComponent(RE.getHtml())

RE.setReadOnly = () => RE.editor.contentEditable = 'false';

RE.setBaseTextColor = color => RE.editor.style.color = color
RE.setBaseFontSize = size => RE.editor.style.fontSize = size
RE.setPadding = (left, top, right, bottom) => {
    RE.editor.style.paddingLeft = left;
    RE.editor.style.paddingTop = top;
    RE.editor.style.paddingRight = right;
    RE.editor.style.paddingBottom = bottom;
}

RE.setBackgroundColor = color => document.body.style.backgroundColor = color
RE.setBackgroundImage = image => RE.editor.style.backgroundImage = image
RE.setWidth = size => RE.editor.style.minWidth = size
RE.setHeight = size => RE.editor.style.height = size
RE.setTextAlign = align => RE.editor.style.textAlign = align
RE.setVerticalAlign = align => RE.editor.style.verticalAlign = align
RE.setPlaceholder = placeholder => RE.editor.setAttribute("placeholder", placeholder)
RE.setInputEnabled = inputEnabled => RE.editor.contentEditable = String(inputEnabled)

RE.undo = () => {
    document.execCommand('undo', false, null);
    RE.enabledEditingItems();
}

RE.redo = () => {
    document.execCommand('redo', false, null);
    RE.enabledEditingItems();
}

RE.setBold = () => {
    document.execCommand('bold', false, null);
    RE.enabledEditingItems();
}

RE.setItalic = () => {
    document.execCommand('italic', false, null);
    RE.enabledEditingItems();
}

RE.setSubscript = () => {
    document.execCommand('subscript', false, null);
    RE.enabledEditingItems();
}

RE.setSuperscript = () => {
    document.execCommand('superscript', false, null);
    RE.enabledEditingItems();
}

RE.setStrikeThrough = () => {
    document.execCommand('strikeThrough', false, null);
    RE.enabledEditingItems();
}

RE.setUnderline = () => {
    document.execCommand('underline', false, null);
    RE.enabledEditingItems();
}

RE.setBullets = () => {
    document.execCommand('insertUnorderedList', false, null);
    RE.enabledEditingItems();
}

RE.setNumbers = () => {
    document.execCommand('insertOrderedList', false, null);
    RE.enabledEditingItems();
}

RE.setTextColor = color => {
    RE.restorerange();
    document.execCommand("styleWithCSS", null, true);
    document.execCommand('foreColor', false, color);
    document.execCommand("styleWithCSS", null, false);
    RE.enabledEditingItems();
}

RE.setTextBackgroundColor = color => {
    RE.restorerange();
    document.execCommand("styleWithCSS", null, true);
    document.execCommand('hiliteColor', false, color);
    document.execCommand("styleWithCSS", null, false);
    RE.enabledEditingItems();
}

RE.setFontSize = fontSize => {
    document.execCommand("fontSize", false, fontSize);
    RE.enabledEditingItems();
}

RE.setHeading = heading => {
    document.execCommand('formatBlock', false, `<h${heading}>`);
    RE.enabledEditingItems();
}

RE.setIndent = () => {
    document.execCommand('indent', false, null);
    RE.enabledEditingItems();
}

RE.setOutdent = () => {
    document.execCommand('outdent', false, null);
    RE.enabledEditingItems();
}

RE.setJustifyLeft = () => {
    document.execCommand('justifyLeft', false, null);
    RE.enabledEditingItems();
}

RE.setJustifyCenter = () => {
    document.execCommand('justifyCenter', false, null);
    RE.enabledEditingItems();
}

RE.setJustifyRight = () => {
    document.execCommand('justifyRight', false, null);
    RE.enabledEditingItems();
}

RE.setBlockquote = () => {
    document.execCommand('formatBlock', false, '<blockquote>');
    RE.enabledEditingItems();
}

RE.insertTab = (src, title, refer, flag) => {
    const html = `<div class="tag" data-flag="${flag}" contenteditable="false">
                        <img src="${src}">
                        <div class="tag-content">
                            <div class="tag-title">${title}</div>
                            <div class="tag-refer">${refer}</div>
                        </div>
                    </div><br>`;
    RE.insertHTML(html);
}

RE.insertImageW = (flag, url, alt, width) => {
    RE.insertHTML(`<img class="insert-img" data-flag="${flag}" src="${url}" alt="${alt}" width="${width}" contenteditable="false"/><br>`);
    RE.enabledEditingItems();
}

RE.insertImageWH = (flag, url, alt, width, height) => {
    RE.insertHTML(`<img class="insert-img" data-flag="${flag}" src="${url}" alt="${alt}" width="${width}" height="${height}" contenteditable="false"/><br>`);
    RE.enabledEditingItems();
}

RE.insertVideo = (url, alt) => {
    RE.insertHTML(`<video src="${url}" controls contenteditable="false" ></video><br>`);
    RE.enabledEditingItems();
}

RE.insertVideoW = (url, width) => {
    RE.insertHTML(`<video src="${url}" width="${width}" controls contenteditable="false"></video><br>`);
    RE.enabledEditingItems();
}

RE.insertVideoWH = (url, width, height) => {
    RE.insertHTML(`<video src="${url}" width="${width}" height="${height}" controls contenteditable="false"></video><br>`);
    RE.enabledEditingItems();
}

RE.insertAudio = (url, alt) => {
    RE.insertHTML(`<audio src="${url}" controls contenteditable="false"></audio><br>`);
    RE.enabledEditingItems();
}

RE.insertHTML = html => {
    RE.restorerange();
    document.execCommand('insertHTML', false, html);
    RE.enabledEditingItems();
}

RE.insertLink = (url, title) => {
    RE.restorerange();
    var sel = document.getSelection();
    if (sel.toString().length == 0) {
        document.execCommand("insertHTML", false, `<a href='${url}' contenteditable="false">${title}</a><br>`);
    } else if (sel.rangeCount) {
        const el = document.createElement("a");
        el.setAttribute("href", url);
        el.setAttribute("title", title);

        const range = sel.getRangeAt(0).cloneRange();
        range.surroundContents(el);
        sel.removeAllRanges();
        sel.addRange(range);
    }
    RE.callback();
}

RE.setTodo = text => {
    document.execCommand('insertHTML', false, `<input type="checkbox"/>`);
    RE.enabledEditingItems();
}

RE.prepareInsert = () => {
    RE.backuprange();
}

RE.backuprange = () => {
    const selection = window.getSelection();
    if (selection.rangeCount > 0) {
        const range = selection.getRangeAt(0);
        RE.currentSelection = {
            "startContainer": range.startContainer,
            "startOffset": range.startOffset,
            "endContainer": range.endContainer,
            "endOffset": range.endOffset
        };
    }
}

RE.restorerange = () => {
    const selection = window.getSelection()
    selection.removeAllRanges()
    const range = document.createRange()
    range.setStart(RE.currentSelection.startContainer, RE.currentSelection.startOffset)
    range.setEnd(RE.currentSelection.endContainer, RE.currentSelection.endOffset)
    selection.addRange(range)
}

RE.enabledEditingItems = () => {
    const items = [];
    if (document.queryCommandState('bold')) items.push('bold')
    if (document.queryCommandState('italic')) items.push('italic')
    if (document.queryCommandState('subscript')) items.push('subscript')
    if (document.queryCommandState('underline')) items.push('underline')
    if (document.queryCommandState('superscript')) items.push('superscript')
    if (document.queryCommandState('justifyFull')) items.push('justifyFull')
    if (document.queryCommandState('justifyLeft')) items.push('justifyLeft')
    if (document.queryCommandState('justifyRight')) items.push('justifyRight')
    if (document.queryCommandState('justifyCenter')) items.push('justifyCenter')
    if (document.queryCommandState('strikeThrough')) items.push('strikeThrough')
    if (document.queryCommandState('insertOrderedList')) items.push('orderedList')
    if (document.queryCommandState('insertUnorderedList')) items.push('unorderedList')
    if (document.queryCommandState('insertHorizontalRule')) items.push('horizontalRule')
    let formatBlock = document.queryCommandValue('formatBlock')
    if (formatBlock.length > 0)
        items.push(formatBlock)

    window.location.href = "re-state://" + encodeURI(items.join(','))
}


RE.focus = () => {
    const range = document.createRange()
    range.selectNodeContents(RE.editor)
    range.collapse(false)
    const selection = window.getSelection()
    selection.removeAllRanges()
    selection.addRange(range)
    RE.editor.focus()
}

RE.blurFocus = () => {
    RE.editor.blur()
}

RE.removeFormat = () => {
    document.execCommand('removeFormat', false, null)
}

RE.callbackEnabledEditingItems = e => {
    if (e.which.toString() === '37' || e.which.toString() === '39') RE.enabledEditingItems()
}

RE.editor.addEventListener("input", RE.callback)
RE.editor.addEventListener("keyup", e => {
    if (e.which.toString() === '37' || e.which.toString() === '39') RE.enabledEditingItems()
})
RE.editor.addEventListener("click", RE.enabledEditingItems)
