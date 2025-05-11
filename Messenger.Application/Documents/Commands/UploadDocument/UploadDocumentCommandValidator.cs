using FluentValidation;

namespace Messenger.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty().WithMessage("Идентификатор чата обязателен");
        RuleFor(x => x.UploaderId).NotEmpty().WithMessage("Идентификатор загрузчика обязателен");
        RuleFor(x => x.File)
            .NotNull().WithMessage("Файл обязателен")
            .Must(file => file.Length > 0).WithMessage("Файл не может быть пустым")
            .Must(file => file.Length <= 15 * 1024 * 1024).WithMessage("Размер файла не должен превышать 15 МБ")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".pdf", ".pptx", ".doc", ".docx", ".rtf", ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            }).WithMessage("Недопустимый тип файла. Допустимые форматы: .pdf, .pptx, .doc, .docx, .rtf, .jpg, .jpeg, .png, .gif");
    }
}