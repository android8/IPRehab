/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />
/// <reference path="../appModels/IUseranswer.ts" />
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html
$(function () {
    pageLoad();
});
function pageLoad() {
    let commandBtnScope = $('.commandBtn');
    $.each($('input[type="checkbox"]', commandBtnScope), function () {
        let $this = $(this);
        if ($this.prop("checked")) {
            setRehabBtns($this.parent());
        }
    });
    //handle rehab action checkbox
    $('input[type="checkbox"]').click(function () {
        let $this = $(this);
        let targetScope = $this.parent();
        if ($this.prop("checked")) {
            setRehabBtns(targetScope);
        }
        else {
            resetRehabBtns(targetScope);
        }
    });
    $('.gotoSection').click(function () {
        let $this = $(this);
        let anchorId = $this.data("anchorid");
        scrollToAnchor(anchorId);
    });
    $("#questionTab").hover(function () {
        $('#questionTab').css({ 'left': '0px', 'transition-duration': '1s' });
    }, function () {
        $('#questionTab').css({ 'left': '-230px', 'transition-duration': '1s' });
    });
}
/* scroll to an anchor */
function scrollToAnchor(aid) {
    let aTag = $('a[name="' + aid + '"]');
    $('html,body').animate({ scrollTop: aTag.offset().top - 10 }, 'fast');
}
function setRehabBtns(targetScope) {
    let currentIdx = 0;
    $.each($('.rehabAction', targetScope), function () {
        let $this = $(this);
        let newTitle = $this.attr('title').replace(/Edit/g, 'Create');
        let newHref = $this.attr('href').replace(/Edit/g, 'Create');
        $this.attr('title', newTitle);
        $this.attr('href', newHref);
        currentIdx++;
        let newClass = $this.attr('class') + ' createActionCmd' + currentIdx.toString();
        ;
        $this.attr('class', newClass);
    });
}
function resetRehabBtns(targetScope) {
    let cmdBtns = ['primary', 'info', 'secondary', 'success', 'warning'];
    let currentIdx = 0;
    $.each($('.rehabAction', targetScope), function () {
        let $this = $(this);
        let newTitle = $this.attr('title').replace(/Create/g, 'Edit');
        let newHref = $this.attr('href').replace(/Create/g, 'Edit');
        $this.attr('title', newTitle);
        $this.attr('href', newHref);
        let resetClass = '';
        resetClass = 'badge badge-' + cmdBtns[currentIdx] + ' rehabAction';
        currentIdx++;
        $this.attr('class', resetClass);
    });
}
export {};
//# sourceMappingURL=form.js.map