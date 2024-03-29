﻿using API.Abstracts;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace API.Controllers.API;

[AuthorizeMiddleware]
[Route("api/[controller]")]
public class StudentController : BasicCrudController<Student>
{
    private readonly IStudentLogic _studentLogic;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="studentLogic"></param>
    public StudentController(IStudentLogic studentLogic)
    {
        _studentLogic = studentLogic;
    }

    /// <summary>
    /// Returns instance of logic
    /// </summary>
    /// <returns></returns>
    protected override IBasicCrudLogic<Student> BasicCrudLogic()
    {
        return _studentLogic;
    }
}