<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="MultidimensionalDataInput.aspx.cs" Inherits="InSite.UI.Admin.Prototypes.MultidimensionalDataInput" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="row">

        <div class="col-lg-6">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <insite:UpdatePanel runat="server" ID="DimensionsUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ApplyDimensionsButton" />
                        </Triggers>
                        <ContentTemplate>
                            <h3 runat="server" id="DimensionsHeader"></h3>

                            <asp:Repeater runat="server" ID="DimensionRepeater">
                                <HeaderTemplate>
                                    <table class="table">
                                        <thead>
                                            <tr>
                                                <th>Name</th>
                                                <th>Values</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <insite:TextBox runat="server" ID="Name" Text='<%# Eval("Name") %>' />
                                        </td>
                                        <td>
                                            <insite:TextBox runat="server" ID="Values" Text='<%# GetDimensionValues() %>' TextMode="MultiLine" Rows="5" />
                                        </td>
                                        <td class="text-nowrap" style="width:0;">
                                            <insite:IconButton runat="server" Name="trash-alt" CommandName="Remove" ToolTip="Remove" ConfirmText="Are you sure you want to remove this dimension?"/>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>

                            <div class="mt-3">
                                <insite:Button runat="server" ID="AddDimensionButton" ButtonStyle="Primary" Icon="fas fa-plus-circle" Text="Add Dimension" />
                                <insite:Button runat="server" ID="ApplyDimensionsButton" ButtonStyle="Success" Icon="fas fa-check" Text="Apply Changes" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </div>

        <div class="col-lg-6">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3>Mapping</h3>

                    <insite:UpdatePanel runat="server" ID="MappingUpdatePanel">
                        <ContentTemplate>
                            <asp:Repeater runat="server" ID="MappingRepeater">
                                <ItemTemplate>
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            <%# Eval("Type") %>
                                        </label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="Dimension" />
                                            <asp:Literal runat="server" ID="Type" Text='<%# Eval("Type") %>' Visible="false" />
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mt-3">
                        <insite:Button runat="server" ID="ApplyMappingButton" ButtonStyle="Success" Icon="fas fa-check" Text="Apply Changes" />
                    </div>

                </div>
            </div>
        </div>

    </div>

    <div runat="server" id="DataSection" class="mt-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div runat="server" id="DataContainer"></div>
                <asp:HiddenField runat="server" ID="MappingInput" />
                <asp:HiddenField runat="server" ID="DataInput" />
            </div>
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (() => {
                const mapping = loadMapping();
                if (!mapping) {
                    document.getElementById('<%= DataSection.ClientID %>').classList.add('d-none');
                    return;
                }

                const renderValueHeader = false;
                const getData = (() => {
                    const body = 'return data' + getDataPath('indexPath') + ';';

                    return Function('data', 'indexPath', body);
                })();
                const setData = (() => {
                    const body = 'data' + getDataPath('indexPath') + ' = value;';

                    return Function('data', 'indexPath', 'value', body);
                })();

                const data = loadData();
                const container = document.getElementById('<%= DataContainer.ClientID %>');
                const templates = Object.freeze({
                    tabs: Object.freeze({
                        header: createTemplate('<h3></h3>'),
                        ul: createTemplate('<ul class="nav nav-tabs" role="tablist"></ul>'),
                        li: createTemplate('<li class="nav-item" role="presentation"></li>'),
                        button: createTemplate('<button class="nav-link" data-bs-toggle="tab" type="button" role="tab"></button>'),
                        content: createTemplate('<div class="tab-content"></div>'),
                        pane: createTemplate('<div class="tab-pane" role="tabpanel" tabindex="0"></div>'),
                    }),
                    table: Object.freeze({
                        header: createTemplate('<h3></h3>'),
                        table: createTemplate('<table class="table"><thead></tbody><tbody></tbody></table>'),
                        rowNameCell: createTemplate('<td class="text-start"></td>'),
                        columnNameCell: createTemplate('<td class="text-center"></td>'),
                    }),
                    value: Object.freeze({
                        header: createTemplate('<h6></h6>'),
                        select: createTemplate('<select class="form-select" multiple></select>')
                    })
                });

                {
                    const indexPath = [];
                    render(container, mapping, 0, indexPath, container.id);
                    if (indexPath.length > 0)
                        throwError('indexPath is not empty');
                }

                function loadMapping() {
                    const value = document.getElementById('<%= MappingInput.ClientID %>')?.value;
                    return value ? JSON.parse(value) : null;
                }

                function loadData() {
                    let value = document.getElementById('<%= DataInput.ClientID %>').value;
                    if (value)
                        return JSON.parse(value);

                    const dimensions = Array(mapping.length);
                    for (let map of mapping)
                        dimensions[map.Dimension.Index] = map.Dimension;

                    const result = Array(dimensions[0].Values.length).fill(0);

                    let currentArrays = [result];

                    for (let i = 1; i < dimensions.length; i++) {
                        const dim = dimensions[i];
                        const newArrays = [];

                        for (let x = 0; x < currentArrays.length; x++) {
                            const cArr = currentArrays[x];
                            for (let y = 0; y < cArr.length; y++) {
                                newArrays.push(cArr[y] = Array(dim.Values.length).fill(0));
                            }
                        }

                        currentArrays = newArrays;
                    }

                    return result;
                }

                function saveData() {
                    document.getElementById('<%= DataInput.ClientID %>').value = JSON.stringify(data);
                }

                function createTemplate(html) {
                    const result = document.createElement('template');
                    result.innerHTML = html;
                    return result;
                }

                function cloneTemplate(el) {
                    return el.content.cloneNode(true).firstChild;
                }

                function throwError(text) {
                    alert(text);
                    throw text;
                }

                function getDataPath(indexPathVar) {
                    const map = Array(mapping.length);

                    for (let i = 0; i < mapping.length; i++) {
                        const dim = mapping[i].Dimension;
                        map[dim.Index] = '[' + String(indexPathVar) + '[' + String(i) + ']]';
                    }

                    return map.join('');
                }

                function onSelectChanged(e) {
                    for (let option of e.currentTarget.options)
                        setData(data, option.indexPath, option.selected ? 1 : 0);
                    saveData();
                }

                // render

                function render(container, mapping, index, indexPath, idPrefix) {
                    container.replaceChildren();

                    const type = mapping[index].Type;
                    if (type == 'Tab')
                        renderNav(container, mapping, index, indexPath, idPrefix);
                    else if (type == 'Column' || type == 'Row')
                        renderTable(container, mapping, index, indexPath, idPrefix);
                    else if (type == 'Value')
                        renderValue(container, mapping, index, indexPath, idPrefix);
                    else
                        throwError('Invalid map type: ' + String(type));
                }

                function renderNav(container, mapping, index, indexPath, idPrefix) {
                    const map = mapping[index];
                    if (map.Type !== 'Tab')
                        throwError('Invalid nav map type: get ' + String(map.Type) + 'while expected Tab');

                    const dimension = map.Dimension;

                    const header = cloneTemplate(templates.tabs.header);
                    header.innerText = dimension.Name;

                    const tablist = cloneTemplate(templates.tabs.ul);
                    const content = cloneTemplate(templates.tabs.content);

                    for (var i = 0; i < dimension.Values.length; i++) {
                        const item = dimension.Values[i];
                        const tabId = idPrefix + '_tab' + String(i);
                        const paneId = tabId + '_pane';

                        const li = cloneTemplate(templates.tabs.li);

                        const button = cloneTemplate(templates.tabs.button);
                        button.id = tabId;
                        button.setAttribute('data-bs-target', '#' + paneId);
                        button.setAttribute('aria-controls', paneId);
                        button.innerText = item;

                        const pane = cloneTemplate(templates.tabs.pane);
                        pane.id = paneId;
                        pane.setAttribute('aria-labelledby', tabId);

                        if (i == 0) {
                            button.classList.add('active');
                            button.setAttribute('aria-selected', 'true');

                            pane.classList.add('show');
                            pane.classList.add('active');
                        }

                        indexPath.push(i);

                        render(pane, mapping, index + 1, indexPath, tabId);

                        indexPath.pop();

                        li.appendChild(button);
                        tablist.appendChild(li);
                        content.appendChild(pane);
                    }

                    container.appendChild(header);
                    container.appendChild(tablist);
                    container.appendChild(content);
                }

                function renderTable(container, mapping, index, indexPath, idPrefix) {
                    const map1 = mapping[index];
                    const map2 = mapping[index + 1];

                    let rowDim = null, colDim = null, colPathIndex = -1, rowPathIndex = -1;

                    if (map1.Type == 'Column') {
                        if (map2.Type == 'Row') {
                            colDim = map1.Dimension;
                            rowDim = map2.Dimension;

                            colPathIndex = indexPath.length;
                            indexPath.push(-1);

                            rowPathIndex = indexPath.length;
                            indexPath.push(-1);
                        } else if (map2.Type == 'Value') {
                            colDim = map1.Dimension;

                            colPathIndex = indexPath.length;
                            indexPath.push(-1);
                        }
                    } else if (map1.Type == 'Row') {
                        if (map2.Type == 'Column') {
                            rowDim = map1.Dimension;
                            colDim = map2.Dimension;

                            rowPathIndex = indexPath.length;
                            indexPath.push(-1);

                            colPathIndex = indexPath.length;
                            indexPath.push(-1);
                        } else if (map2.Type == 'Value') {
                            rowDim = map1.Dimension;

                            rowPathIndex = indexPath.length;
                            indexPath.push(-1);
                        }
                    }

                    if (!rowDim && !colDim)
                        throwError('Invalid table map type: ' + String(map1.Type) + ', ' + String(map2.Type));

                    const table = cloneTemplate(templates.table.table);
                    const tHead = table.querySelector('thead');
                    const tBody = table.querySelector('tbody');

                    if (colDim) {
                        if (rowDim) {
                            const row = document.createElement('tr');

                            const rowCell = cloneTemplate(templates.table.rowNameCell);
                            rowCell.innerText = rowDim.Name;

                            const colCell = cloneTemplate(templates.table.columnNameCell);
                            colCell.setAttribute('colspan', colDim.Values.length);
                            colCell.innerText = colDim.Name;

                            row.appendChild(rowCell);
                            row.appendChild(colCell);
                            tHead.appendChild(row);
                        } else {
                            const header = cloneTemplate(templates.table.header);
                            header.innerText = colDim.Name;
                            container.appendChild(header);
                        }

                        {
                            const row = document.createElement('tr');

                            if (rowDim) {
                                const rowCell = document.createElement('th');
                                row.appendChild(rowCell);
                            }

                            for (let col of colDim.Values) {
                                const colCell = document.createElement('th');
                                colCell.innerText = col;
                                row.appendChild(colCell);
                            }

                            tHead.appendChild(row);
                        }

                        if (rowDim) {
                            for (let ri = 0; ri < rowDim.Values.length; ri++) {
                                const row = document.createElement('tr');

                                const rowCell = document.createElement('th');
                                rowCell.innerText = rowDim.Values[ri];
                                row.appendChild(rowCell);

                                indexPath[rowPathIndex] = ri;
                                renderBodyColumns(row, index + 1, idPrefix + '_row' + String(ri));

                                tBody.appendChild(row);
                            }
                        } else {
                            const row = document.createElement('tr');
                            renderBodyColumns(row, index, idPrefix);
                            tBody.appendChild(row);
                        }

                        function renderBodyColumns(row, index, idPrefix) {
                            for (let ci = 0; ci < colDim.Values.length; ci++) {
                                const colCell = document.createElement('td');
                                indexPath[colPathIndex] = ci;
                                render(colCell, mapping, index + 1, indexPath, idPrefix + '_col' + String(ci))
                                row.appendChild(colCell);
                            }
                        }
                    } else {
                        tHead.remove();

                        const header = cloneTemplate(templates.table.header);
                        header.innerText = rowDim.Name;
                        container.appendChild(header);

                        for (let ri = 0; ri < rowDim.Values.length; ri++) {
                            const row = document.createElement('tr');

                            const rowCell = document.createElement('th');
                            rowCell.innerText = rowDim.Values[ri];
                            row.appendChild(rowCell);

                            indexPath[rowPathIndex] = ri;
                            const valueCell = document.createElement('td');
                            render(valueCell, mapping, index + 1, indexPath, idPrefix + '_row' + String(ri))
                            row.appendChild(valueCell);

                            tBody.appendChild(row);
                        }
                    }

                    if (colPathIndex >= 0)
                        indexPath.pop();

                    if (rowPathIndex >= 0)
                        indexPath.pop();

                    container.appendChild(table);
                }

                function renderValue(container, mapping, index, indexPath, idPrefix) {
                    const map = mapping[index];
                    if (map.Type !== 'Value')
                        throw 'Invalid map type: ' + String(map.Type);

                    if (renderValueHeader) {
                        const header = cloneTemplate(templates.value.header);
                        header.innerText = map.Dimension.Name;
                        container.appendChild(header);
                    }

                    const select = cloneTemplate(templates.value.select);
                    select.size = map.Dimension.Values.length;

                    for (let i = 0; i < map.Dimension.Values.length; i++) {
                        const option = document.createElement('option');
                        option.innerText = map.Dimension.Values[i];
                        option.indexPath = indexPath.slice();
                        option.indexPath.push(i);
                        option.selected = getData(data, option.indexPath) == 1;
                        select.appendChild(option);
                    }

                    container.appendChild(select);

                    select.addEventListener('change', onSelectChanged);
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>