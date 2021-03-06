using Microsoft.AspNetCore.Mvc;
using Resourcey.Models;
using Resourcey.Data; 
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Resourcey.Controllers
{

[Authorize]
public class ClassroomsController : Controller
  {
    
    private readonly ApplicationDbContext _db; 
    private readonly UserManager <ApplicationUser> _userManager;

    public ClassroomsController (UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
      _userManager = userManager; 
      _db = db;
    }

    public ActionResult Index ()
    {     
      return View (_db.Classrooms.ToList());
    }
    public async Task <ActionResult> YourClassrooms () //specific to the user
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userClassroom = _db.Classrooms.Where(entry => entry.ClassroomCreator.Id == currentUser.Id);
      return View (userClassroom);
    }

    public ActionResult Create()
    {
      return View (); 
    }

    [HttpPost]
    public async Task<ActionResult> Create (Classroom classroom)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
      var currentUser = await _userManager.FindByIdAsync(userId);
      classroom.ClassroomCreator = currentUser; 
      _db.Classrooms.Add(classroom);
      _db.SaveChanges(); 
      return RedirectToAction("Index");
    }
    
    public async Task<ActionResult> Details(int id) 
    {
      var thisClassroom = _db.Classrooms
      .Include(classroom => classroom.Sections)
      .FirstOrDefault(classroom => classroom.ClassroomId == id);
      TempData["classroomId"] = id;
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      if(currentUser == thisClassroom.ClassroomCreator)
      {
        return View("Details", thisClassroom);
      }
      else 
      {
      return View("StudentDetails", thisClassroom);
      }
    }

    public async Task<ActionResult> Edit(int id) 
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisClassroom = _db.Classrooms.FirstOrDefault(classroom => classroom.ClassroomId == id);
      if(currentUser == thisClassroom.ClassroomCreator)
      {
        return View(thisClassroom);
      }
      else
      {
        return RedirectToAction("Index");
      }
    }

    [HttpPost]
    public ActionResult Edit (Classroom classroom)
    {
      _db.Entry(classroom).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task <ActionResult> Delete(int id) 
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisClassroom = _db.Classrooms.FirstOrDefault(classroom => classroom.ClassroomId == id);
      if(currentUser == thisClassroom.ClassroomCreator)
      {
        return View(thisClassroom);
      }
      else
      {
        return RedirectToAction("Index");
      }
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisClassroom = _db.Classrooms.FirstOrDefault(classrooms => classrooms.ClassroomId == id);
      _db.Classrooms.Remove(thisClassroom);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}