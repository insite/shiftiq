<%@ Page Language="C#" CodeBehind="Combine.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.Combine" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .merge-item {
            transition: background-color .25s;
            cursor: pointer;
        }

        .merge-value {
            white-space: pre-wrap;
            padding-left: 1.7rem;
        }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <div class="row mb-3">
        <div class="col-md-6 mb-3 mb-md-0">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">Person 1</h4>

                    <asp:Repeater runat="server" ID="ValueRepeater1">
                        <ItemTemplate>
                            <div class="mt-3 merge-1">
                                <h6 class="mb-2">
                                    <%# Eval("Name") %>
                                    <span runat="server" visible='<%# Eval("IsSame") %>' class="ms-2 badge bg-success float-end">Same</span>
                                </h6>

                                <div class="clearfix merge-item">
                                    <div class="float-start merge-input">
                                        <insite:RadioButton runat="server" ID="Selected" Visible='<%# !(bool)Eval("IsSame") %>'
                                            GroupName='<%# "merge_row_" + Container.ItemIndex %>' Layout="Input" />
                                    </div>
                                    
                                    <div class="merge-value"><%# Eval("Value1") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate><hr /></SeparatorTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </div>
        <div class="col-md-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">Person 2</h4>

                    <asp:Repeater runat="server" ID="ValueRepeater2">
                        <ItemTemplate>
                            <div class="mt-3 merge-2">
                                <h6 class="mb-2">
                                    <%# Eval("Name") %>
                                    <span runat="server" visible='<%# Eval("IsSame") %>' class="ms-2 badge bg-success float-end">Same</span>
                                </h6>

                                <div class="clearfix merge-item">
                                    <div class="float-start merge-input">
                                        <insite:RadioButton runat="server" ID="Selected" Visible='<%# !(bool)Eval("IsSame") %>'
                                            GroupName='<%# "merge_row_" + Container.ItemIndex %>' Layout="Input" />
                                    </div>
                                    
                                    <div class="merge-value"><%# Eval("Value2") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate><hr /></SeparatorTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </div>
    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:Button runat="server" ID="CombineButton"
                CssClass="disabled"
                Text="Combine"
                Icon="fas fa-cloud-upload"
                ButtonStyle="Success"
            />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var $combineButton = $('#<%= CombineButton.ClientID %>').on('click', function (e) {
                    if ($(this).hasClass('disabled') === true || !confirm('Are you sure to combine these people?')) {
                        e.preventDefault();
                        e.stopPropagation();
                    } else {
                        this.classList.add("disabled");
                    }
                });

                var $merges1 = $('.merge-1')
                    .on('mouseenter', onMouseEnterMerge1)
                    .on('mouseleave', onMouseLeaveMerge1);
                var $merges2 = $('.merge-2')
                    .on('mouseenter', onMouseEnterMerge2)
                    .on('mouseleave', onMouseLeaveMerge2);

                var $items1 = $('.merge-1 .merge-item')
                    .on('click', onClickItem);
                var $items2 = $('.merge-2 .merge-item')
                    .on('click', onClickItem);

                var $radios1 = $('.merge-1 .merge-item input[type="radio"]')
                    .on('click', onClickRadio)
                    .on('change', onChangeRadio);
                var $radios2 = $('.merge-2 .merge-item input[type="radio"]')
                    .on('click', onClickRadio)
                    .on('change', onChangeRadio);

                onChangeRadio();
                onWindowResize();

                $(document).ready(onWindowResize);

                $(window).on('resize', onWindowResize);

                function onMouseEnterMerge1() {
                    onMouseEnterItems($merges1.index(this));
                }

                function onMouseLeaveMerge1() {
                    onMouseLeaveItems($merges1.index(this));
                }

                function onMouseEnterMerge2() {
                    onMouseEnterItems($merges2.index(this));
                }

                function onMouseLeaveMerge2() {
                    onMouseLeaveItems($merges2.index(this));
                }

                function onMouseEnterItems(index) {
                    $items1.eq(index).addClass('bg-secondary');
                    $items2.eq(index).addClass('bg-secondary');
                }

                function onMouseLeaveItems(index) {
                    $items1.eq(index).removeClass('bg-secondary');
                    $items2.eq(index).removeClass('bg-secondary');
                }

                function onWindowResize() {
                    $items1.css('min-height', '');
                    $items2.css('min-height', '');

                    for (var i = 0; i < $items1.length; i++) {
                        var $item1 = $items1.eq(i);
                        var $item2 = $items2.eq(i);
                        var height1 = $item1.height();
                        var height2 = $item2.height();

                        if (height1 > height2)
                            $item2.css('min-height', String(height1) + 'px');
                        else if (height2 > height1)
                            $item1.css('min-height', String(height2) + 'px');
                    }
                }

                function onClickItem() {
                    $(this).find('input[type="radio"]').click();
                }

                function onClickRadio(e) {
                    e.stopPropagation();
                }

                function onChangeRadio() {
                    var totalCount = ($radios1.length + $radios2.length) / 2;
                    var checkedCount = $radios1.filter(':checked').length + $radios2.filter(':checked').length;

                    if (totalCount == checkedCount)
                        $combineButton.removeClass('disabled');
                    else
                        $combineButton.addClass('disabled');
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>