using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IT15_LabExam_Tero.Data;
using IT15_LabExam_Tero.Models;

namespace IT15_LabExam_Tero.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly AppDbContext _context;

        // The constructor that sets up the database context
        public PayrollsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Payrolls
        public async Task<IActionResult> Index()
        {
            // Includes Employee data so you can show First/Last name in the table
            var appDbContext = _context.Payrolls.Include(p => p.Employee);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);

            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // GET: Payrolls/History/5
        public async Task<IActionResult> History(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var payrolls = await _context.Payrolls
                .Where(p => p.EmployeeId == id)
                .OrderByDescending(p => p.Date)
                .Include(p => p.Employee)
                .ToListAsync();

            ViewData["EmployeeName"] = $"{employee.FirstName} {employee.LastName}";
            ViewData["EmployeeId"] = id;
            return View(payrolls);
        }

        // GET: Payrolls/Create
        public IActionResult Create(int? employeeId)
        {
            // Sets up the dropdown menu before the page loads
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", employeeId);
            return View();
        }

        // POST: Payrolls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayrollId,EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Please select a valid employee.");
                }
                else
                {
                    ApplyPayrollCalculations(payroll, employee.DailyRate);
                    if (payroll.NetPay < 0)
                    {
                        ModelState.AddModelError("Deduction", "Deduction cannot be greater than gross pay.");
                    }
                    else
                    {
                        _context.Add(payroll);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Repopulate dropdown if validation fails
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", payroll.EmployeeId);
            return View(payroll);
        }

        // POST: Payrolls/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
        {
            if (id != payroll.PayrollId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Please select a valid employee.");
                }
                else
                {
                    ApplyPayrollCalculations(payroll, employee.DailyRate);
                    if (payroll.NetPay < 0)
                    {
                        ModelState.AddModelError("Deduction", "Deduction cannot be greater than gross pay.");
                    }
                    else
                    {
                        try
                        {
                            _context.Update(payroll);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PayrollExists(payroll.PayrollId))
                            {
                                return NotFound();
                            }

                            throw;
                        }

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);

            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private static void ApplyPayrollCalculations(Payroll payroll, decimal dailyRate)
        {
            payroll.GrossPay = payroll.DaysWorked * dailyRate;
            payroll.NetPay = payroll.GrossPay - payroll.Deduction;
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}