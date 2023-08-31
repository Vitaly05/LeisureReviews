var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
$('#save-review-button').on('click', function (e) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    e.preventDefault();
                    if (!$('#edit-review-form').valid()) return [3 /*break*/, 2];
                    $(this).prop('disabled', true);
                    $(this).find('#button-text').hide();
                    $(this).find('#button-spinner').show();
                    return [4 /*yield*/, saveReview()];
                case 1: return [2 /*return*/, _a.sent()];
                case 2: return [2 /*return*/];
            }
        });
    });
});
$('.review-field').on('input', function () {
    $("[name=\"Review.".concat($(this).data('field'), "\"]")).val($(this).val());
});
//@ts-ignore
var tagsInput = new Tokenfield({
    el: document.querySelector('#tags-input'),
    items: getTagsInputItems('#used-tags'),
    setItems: getTagsInputItems('#tags-input'),
    minChars: 0,
    delimiters: [',']
});
function getTagsInputItems(selector) {
    var tagsInputValues = $(selector).val().split(',');
    return tagsInputValues.every(function (v) { return v.length == 0; }) ? [] : tagsInputValues.map(function (v, i) { return ({ id: i++, name: v }); });
}
function saveReview() {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, $.ajax({
                        url: '/Review/Save',
                        type: 'POST',
                        data: getData(),
                        processData: false,
                        contentType: false
                    }).always(function () {
                        hideSpinner();
                        $('.validation-summary-errors ul').empty();
                    }).done(function (data) {
                        $('[name="Review.Id"]').val(data.id);
                        $('[name="Review.AuthorId"]').val(data.authorId);
                        //@ts-ignore
                        UIkit.modal($('#successful-save-modal')).show();
                    })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
function hideSpinner() {
    $('#save-review-button').prop('disabled', false);
    $('#save-review-button').find('#button-text').show();
    $('#save-review-button').find('#button-spinner').hide();
}
function getData() {
    var formData = new FormData();
    formData.append('title', $('[name="Review.Title"]').val());
    formData.append('leisure', $('[name="Review.Leisure"]').val());
    formData.append('group', $('[name="Review.Group"]').val());
    formData.append('authorRate', $('[name="Review.AuthorRate"]').val());
    formData.append('content', $('[name="Review.Content"]').val());
    formData.append('authorId', $('[name="Review.AuthorId"]').val());
    var tagsNames = tagsInput.getItems().map(function (i) { return i.name; });
    tagsNames.length === 0 || tagsNames.forEach(function (t) { return formData.append('tagsNames[]', t); });
    formData.append('id', $('[name="Review.Id"]').val());
    formData.append('createTime', $('[name="Review.CreateTime"]').val());
    formData.append('illustration', $('#illustration-file-input').prop('files')[0]);
    return formData;
}
//# sourceMappingURL=reviewEditor.js.map