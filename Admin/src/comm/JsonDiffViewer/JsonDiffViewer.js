class JsonDiffViewer {
    constructor(config = {}) {
        this.validateConfig(config);
        this.mode = this.normalizeMode(config.mode)
        this.onLeftUpdate = config.onLeftUpdate || (() => {
        })
        this.onRightUpdate = config.onRightUpdate || (() => {
        })
        this.leftData = null;
        this.rightData = null;
        this.diff = {};
    }

    static diffJson(a, b, mode = {'*': 'strict'}) {
        const modeConfig = this.parseModeConfig(mode);
        const diffs = {};
        const initialMode = modeConfig['*'] || 'strict';
        JsonDiffViewer.compare(a, b, '', diffs, modeConfig, initialMode);
        return diffs;
    }

    // 新增配置处理逻辑
    static parseModeConfig(mode) {
        if (typeof mode === 'string') {
            console.warn('字符串模式配置已废弃，请使用对象格式');
            return this.convertLegacyMode(mode);
        }
        return {...mode};
    }

    static convertLegacyMode(legacyMode) {
        const modeConfig = {'*': 'strict'};
        legacyMode.split(',').forEach(part => {
            const [path, mode] = part.split(':').map(s => s.trim());
            if (path && mode) modeConfig[path] = mode;
        });
        return modeConfig;
    }

    normalizeMode(mode) {
        if (!mode) return {'*': 'strict'};
        if (typeof mode === 'string') {
            throw new Error('模式配置必须使用对象格式，示例: {\'*\': \'strict\', Steps: \'loose\'}');
        }
        return {...mode, '*': mode['*'] || 'strict'};
    }

    validateConfig(config) {
        if (config.mode && typeof config.mode === 'string') {
            throw new Error('非法配置格式：请使用对象格式配置模式，示例: {\'*\': \'strict\', Steps: \'loose\'}');
        }
    }

    setLJson(data) {
        this.leftData = data;
        this.updateDiff();
        this.renderBoth();
    }

    setRJson(data) {
        this.rightData = data;
        this.updateDiff();
        this.renderBoth();
    }

    updateDiff() {
        if (this.leftData && this.rightData) {
            const mode = this.mode || 'strict';
            this.diff = JsonDiffViewer.diffJson(this.leftData, this.rightData, mode);
        } else {
            this.diff = {};
        }
    }

    renderBoth() {
        this.renderLeft();
        this.renderRight();
    }

    static initCollapsibleHover() {
        document.querySelectorAll('.json-collapsible').forEach(collapsible => {
            let hideTimer;
            collapsible.addEventListener('mouseenter', () => {
                const closeButton = collapsible.querySelector('.json-toggle-close');
                if (closeButton) {
                    clearTimeout(hideTimer);
                    closeButton.style.display = 'inline';
                }
            });
            collapsible.addEventListener('mouseleave', () => {
                const closeButton = collapsible.querySelector('.json-toggle-close');
                if (closeButton) {
                    hideTimer = setTimeout(() => {
                        closeButton.style.display = 'none';
                    }, 300);
                }
            });
        });
    }

    renderLeft() {
        const html = this.leftData ? JsonDiffViewer.syntaxHighlight(this.leftData, this.diff, 'left') : '';
        this.onLeftUpdate(html);
        JsonDiffViewer.initCollapsibleHover()
    }

    renderRight() {
        const html = this.rightData ? JsonDiffViewer.syntaxHighlight(this.rightData, this.diff, 'right') : '';
        this.onRightUpdate(html);
        JsonDiffViewer.initCollapsibleHover()
    }


    static compare(a, b, path, diffs, modeConfig, currentMode) {
        const pathMode = modeConfig[path];
        const effectiveMode = pathMode !== undefined ? pathMode : currentMode;

        if (typeof a !== typeof b) {
            diffs[path] = {type: 'structure'};
            return;
        }

        if (a === null || b === null) {
            if (a !== b) diffs[path] = {type: 'structure'};
            return;
        }

        if (typeof a === 'object') {
            if (Array.isArray(a) !== Array.isArray(b)) {
                diffs[path] = {type: 'structure'};
                return;
            }

            if (Array.isArray(a)) {
                if (effectiveMode === 'strict') {
                    JsonDiffViewer.strictCompareArray(a, b, path, diffs, modeConfig, effectiveMode);
                } else {
                    JsonDiffViewer.looseCompareArray(a, b, path, diffs, modeConfig, effectiveMode);
                }
            } else {
                const allKeys = new Set([...Object.keys(a), ...Object.keys(b)]);
                allKeys.forEach(key => {
                    const keyPath = path ? `${path}.${key}` : key;
                    if (!(key in a)) {
                        diffs[keyPath] = {type: 'field'};
                    } else if (!(key in b)) {
                        diffs[keyPath] = {type: 'field'};
                    } else {
                        JsonDiffViewer.compare(a[key], b[key], keyPath, diffs, modeConfig, effectiveMode);
                    }
                });
            }
        } else if (a !== b) {
            diffs[path] = {type: 'value'};
        }
    }

    static strictCompareArray(a, b, path, diffs, modeConfig, currentMode) {
        const maxLen = Math.max(a.length, b.length);
        for (let i = 0; i < maxLen; i++) {
            const elemPath = `${path}[${i}]`;
            if (i >= a.length || i >= b.length) {
                diffs[elemPath] = {type: 'structure'};
            } else {
                JsonDiffViewer.compare(a[i], b[i], elemPath, diffs, modeConfig, currentMode);
            }
        }
    }

    static looseCompareArray(a, b, path, diffs, modeConfig, currentMode) {
        const listA = a.map((v, i) => ({index: i, value: v}));
        const listB = b.map((v, i) => ({index: i, value: v}));
        const matches = [];
        const usedB = new Set();

        for (const aElem of listA) {
            let found = false;
            for (const bElem of listB) {
                if (!usedB.has(bElem.index) && JsonDiffViewer.deepEqual(aElem.value, bElem.value)) {
                    matches.push({aIndex: aElem.index, bIndex: bElem.index});
                    usedB.add(bElem.index);
                    found = true;
                    break;
                }
            }
            if (!found) {
                const elemPath = `${path}[${aElem.index}]`;
                if (aElem.index >= b.length) {
                    diffs[elemPath] = {type: 'structure'};
                } else {
                    JsonDiffViewer.compare(aElem.value, b[aElem.index], elemPath, diffs, modeConfig, currentMode);
                }
            }
        }

        for (const bElem of listB) {
            if (!usedB.has(bElem.index)) {
                const elemPath = `${path}[${bElem.index}]`;
                if (bElem.index >= a.length) {
                    diffs[elemPath] = {type: 'structure'};
                } else if (!diffs[elemPath]) {
                    JsonDiffViewer.compare(a[bElem.index], bElem.value, elemPath, diffs, modeConfig, currentMode);
                }
            }
        }

        for (const match of matches) {
            const aPath = `${path}[${match.aIndex}]`;
            const bPath = `${path}[${match.bIndex}]`;

            if (match.aIndex !== match.bIndex) {
                diffs[aPath] = {...diffs[aPath], order: match.bIndex};
                diffs[bPath] = {...diffs[bPath], order: match.aIndex};
            }

            JsonDiffViewer.compare(a[match.aIndex], b[match.bIndex], aPath, diffs, modeConfig, currentMode);
        }
    }

    static deepEqual(a, b) {
        if (typeof a !== typeof b) return false;
        if (a === null || b === null) return a === b;
        if (typeof a === 'object') {
            if (Array.isArray(a) !== Array.isArray(b)) return false;
            if (Array.isArray(a)) {
                if (a.length !== b.length) return false;
                return a.every((v, i) => JsonDiffViewer.deepEqual(v, b[i]));
            }
            const aKeys = Object.keys(a), bKeys = Object.keys(b);
            if (aKeys.length !== bKeys.length) return false;
            return aKeys.every(k => bKeys.includes(k) && JsonDiffViewer.deepEqual(a[k], b[k]));
        }
        return a === b;
    }

    static syntaxHighlight(data, diffs, side) {
        return `<pre class="json-view">${JsonDiffViewer.recursiveRender(data, '', diffs, side, 0, 0)}</pre>`;
    }

    static recursiveRender(data, path, diffs, side, indent = 0, level = 0) {
        const indentStr = '  '.repeat(indent);
        let html = '';

        if (typeof data === 'object' && data !== null) {
            const braceColor = JsonDiffViewer.getRainbowColor(level);
            html += `<span class="json-collapsible" data-type="${Array.isArray(data) ? 'array' : 'object'}" data-collapsed="false">`;
            html += `<span class="json-toggle-close" onclick="JsonDiffViewer.toggleCollapse(this)">➖</span>`;
            html += `<span class="json-content" style="display:inline;">`;

            if (Array.isArray(data)) {
                html += `<span class="json-punctuation" style="color: ${braceColor}">[</span>\n`;
                data.forEach((item, index) => {
                    const elemPath = `${path}[${index}]`;
                    const diff = diffs[elemPath] || {};
                    const isDiff = ['structure', 'value', 'field'].includes(diff.type);

                    const content = JsonDiffViewer.recursiveRender(item, elemPath, diffs, side, indent + 1, level + 1);
                    let orderMarker = diff.order === undefined ? '' : `<span class="order-marker">${side === 'left' ? `${index} → ${diff.order}` : `${diff.order} → ${index}`}</span>`
                    const comma = index < data.length - 1 ? '<span class="json-punctuation">,</span>\n' : '\n';
                    html += `${indentStr}  ${JsonDiffViewer.wrapDiff(isDiff, diff.type, () => `${orderMarker}${content}${comma}`)}`;
                });
                html += `${indentStr}<span class="json-punctuation" style="color: ${braceColor}">]</span>`;
            } else {
                html += `<span class="json-punctuation" style="color: ${braceColor}">{</span>\n`;
                Object.entries(data).forEach(([key, value], index) => {
                    const elemPath = path ? `${path}.${key}` : key;
                    const diff = diffs[elemPath] || {};
                    const isDiff = diff.type === 'field';

                    const content = JsonDiffViewer.recursiveRender(value, elemPath, diffs, side, indent + 1, level + 1);
                    const keyContent = `<span class="json-key">"${key}"</span><span class="json-punctuation">:</span> ${content}`;
                    const comma = index < Object.keys(data).length - 1 ? '<span class="json-punctuation">,</span>\n' : '\n';

                    html += `${indentStr}  ${JsonDiffViewer.wrapDiff(isDiff, 'field', () => `${keyContent}${comma}`)}`;
                });
                html += `${indentStr}<span class="json-punctuation" style="color: ${braceColor}">}</span>`;
            }

            html += `</span></span>`;
        } else {
            const diff = diffs[path] || {};
            const isDiff = ['value', 'structure'].includes(diff.type);
            html += JsonDiffViewer.wrapDiff(isDiff, diff.type, () => {
                if (typeof data === 'string' && data.includes('http://localhost:5256/api/File/GetImageFile/files/')) {
                    return `<img src="${data}" alt="" style="height:100px;"/>`;
                } else {
                    const val = typeof data === 'string' ? `"${JsonDiffViewer.escapeHtml(data)}"` : data;
                    const cls = JsonDiffViewer.getValueClass(data);
                    return `<span class="${cls}">${val}</span>`;
                }
            });
        }

        return html;
    }

    static toggleCollapse(toggleElement) {
        const collapsible = toggleElement.parentElement;
        const content = collapsible.querySelector('.json-content');
        const isCollapsed = collapsible.getAttribute('data-collapsed') === 'true';
        const type = collapsible.getAttribute('data-type');

        if (isCollapsed) {
            content.style.display = 'inline';
            collapsible.setAttribute('data-collapsed', 'false');
            toggleElement.innerHTML = '➖';
            toggleElement.className = 'json-toggle-close';
        } else {
            content.style.display = 'none';
            collapsible.setAttribute('data-collapsed', 'true');
            toggleElement.innerHTML = type === 'array' ? '[...]' : '{...}';
            toggleElement.className = 'json-toggle';
        }
    }

    static getRainbowColor(level) {
        const hue = (level * 40) % 360;
        return `hsl(${hue}, 70%, 45%)`;
    }

    static escapeHtml(unsafe) {
        return unsafe?.toString()
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;") || '';
    }

    static wrapDiff(condition, type, contentFn) {
        if (condition) {
            return `<span class="diff-${type}">${contentFn()}</span>`;
        }
        return contentFn();
    }

    static getValueClass(value) {
        if (value === null) return 'json-null';
        switch (typeof value) {
            case 'string':
                return 'json-string';
            case 'number':
                return 'json-number';
            case 'boolean':
                return 'json-boolean';
            default:
                return '';
        }
    }
}