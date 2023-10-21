﻿using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Nexus.Common.Attributes;
using Nexus.CompanyAPI.Abstractions;
using Nexus.CompanyAPI.Data;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Exceptions;

namespace Nexus.CompanyAPI.Services;

/// <summary>
///     Service for managing tags.
/// </summary>
[NexusService<ITagService>(NexusServiceLifeTime.Scoped)]
public class TagService : ITagService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IValidator<Tag> _tagValidator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TagService" /> class.
    /// </summary>
    /// <param name="unitOfWork">The UnitOfWork object containing the repositories.</param>
    /// <param name="tagValidator">The validator for tags</param>
    public TagService(UnitOfWork unitOfWork, IValidator<Tag> tagValidator)
    {
        _unitOfWork = unitOfWork;
        _tagValidator = tagValidator;
    }

    /// <summary>
    ///     Creates a new tag asynchronously.
    /// </summary>
    /// <param name="name">The name of the tag to create.</param>
    /// <returns>The created tag.</returns>
    public async Task<Result<Tag>> CreateAsync(string name)
    {
        ValidationResult? validationResult = await _tagValidator.ValidateAsync(new Tag((name)));

        if (!validationResult.IsValid)
        {
            return new Result<Tag>(new ValidationException(validationResult.Errors));
        }
        
        if (await _unitOfWork.Tags.ExistsWithNameAsync(name))
        {
            return new Result<Tag>(new AnotherTagExistsWithSameNameException(name));
        }
        
        Tag tagToCreate = new (name);
        _unitOfWork.Tags.Add(tagToCreate);
        
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();

        return tagToCreate;
    }

    /// <summary>
    ///     Deletes a tag asynchronously.
    /// </summary>
    /// <param name="name">The name of the tag to delete.</param>
    /// <returns>True if the tag was deleted, false otherwise.</returns>
    public async Task<Result<bool>> DeleteAsync(string name)
    {
        if (await _unitOfWork.Companies.AnyCompaniesExistWithTagName(name))
        {
            return new Result<bool>(new CompanyExistsWithTagNameException(name));
        }
        
        Tag? tagToDelete = await _unitOfWork.Tags.GetByNameAsync(name);
        
        if (tagToDelete != null)
        {
            _unitOfWork.Tags.Delete(tagToDelete);
            _unitOfWork.BeginTransaction();
            _unitOfWork.Commit();
        }
        
        return true;
    }

    /// <summary>
    ///     Gets all tags asynchronously.
    /// </summary>
    /// <returns>A list of all tags.</returns>
    public async Task<List<Tag>> GetAllAsync()
    {
        return await _unitOfWork.Tags.AllAsync();
    }

    public async Task<Result<Tag>> GetByIdAsync(int id)
    {
        Tag? tag = await _unitOfWork.Tags.GetByIdAsync(id);

        if (tag == null)
        {
            return new Result<Tag>(new TagNotFoundException(id));
        }

        return tag;
    }
}