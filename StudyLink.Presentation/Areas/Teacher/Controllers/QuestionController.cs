﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using StudyLink.Presentation.Helpers;
using System;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Areas.Admin.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IAnswerService _answerService;

        public QuestionController(IQuestionService questionService, IQuestionTypeService questionTypeService, IAnswerService answerService)
        {
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
        }

        public async Task<IActionResult> Index(int questionTypeId)
        {
            try
            {
                if (questionTypeId > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", questionTypeId.ToString());
                }
                int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
                questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var marksDifference = await _questionTypeService.GetQuestionMarksDifferenceFromFullMarks(questionTypeId, subjectId);

                if (marksDifference == 0)
                {
                    TempData["MarksDifferenceMessage"] = "No more questions can be added as the full marks have already been covered.";
                }
                else
                {
                    TempData["MarksDifferenceMessage"] = $"There are still {marksDifference} marks left to be completed for this question type.";
                }
                var questions = await _questionService.GetListForAddQuestion(questionTypeId);
                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> QuestionTypes(int subjectId)
        {
            if (subjectId > 0)
            {
                HttpContext.Session.SetString("SubjectId", subjectId.ToString());
            }
             subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
            var questionTypes = await _questionTypeService.GetListBySubjectId(subjectId);
            return View(questionTypes);
        }

        public async Task<IActionResult> Create(int questionTypeId)
        {
            try
            {
                if (questionTypeId > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", questionTypeId.ToString());
                }
                int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
                questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var marksDifference = await _questionTypeService.GetQuestionMarksDifferenceFromFullMarks(questionTypeId, subjectId);
                ViewBag.MarksDifference = marksDifference;

                var questionType = await _questionTypeService.GetById(questionTypeId);
                ViewBag.FullMarks = questionType.FullMarks;
                if (marksDifference == 0)
                {
                    TempData["MarksDifferenceMessage"] = "No more questions can be added as the full marks have already been covered.";
                }
                else
                {
                    TempData["MarksDifferenceMessage"] = $"There are still {marksDifference} marks left to be completed for this question type.";
                }
                var questions = await _questionService.GetListForAddQuestion(questionTypeId);
                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(List<AddQuestionVM> addQuestionVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _questionService.Add(addQuestionVm);

                    if (result == -1)
                    {
                        TempData["Error"] = "Entered marks exceed the full marks.";
                        return View(addQuestionVm);
                    }

                    TempData["Success"] = "Question created successfully!";
                    return RedirectToAction(nameof(QuestionTypes));
                }
                return View(addQuestionVm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = "An error occurred while creating the question.";
                return View(addQuestionVm);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _questionService.GetById(id);
                return View(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   var result = await _questionService.Update(question);
                    if (result == -1)
                    {
                        TempData["Error"] = "Entered marks exceed the full marks.";
                        return View(question);
                    }
                    TempData["Success"] = "Question updated successfully!";
                    return RedirectToAction(nameof(Index), new { questionTypeId = question.QuestionTypeId});

                }
                return View(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return View(question);
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Question question)
        {
            try
            {
                await _questionService.Delete(question.QuestionId);
                TempData["Success"] = "Question deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetStudentResults(int questionTypeId)
        {
            try
            {
                var questions = await _answerService.GetStudentResultByQuestionTypeId(questionTypeId);
                return View(questions);
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> GetQuestionListForView(int questionTypeId)
        {
            try
            {
                if (questionTypeId > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", questionTypeId.ToString());
                }
                int subjectId = int.Parse(HttpContext.Session.GetString("SubjectId"));
                questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var questions = await _questionService.GetList(questionTypeId);
               return Json(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                return this.Handle(ex.Message);
            }
        }

        public async Task<IActionResult> GetResultDetail(int studentId, int questionTypeId)
        {
            try
            {
                if (questionTypeId > 0)
                {
                    HttpContext.Session.SetString("QuestionTypeId", questionTypeId.ToString());
                }
                questionTypeId = int.Parse(HttpContext.Session.GetString("QuestionTypeId"));
                var result = await _answerService.GetResultAsync(studentId);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
