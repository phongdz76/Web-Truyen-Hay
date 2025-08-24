using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    public class UserController : Controller
    {
        truyenhayEntities7 truyen = new truyenhayEntities7();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(NguoiDung _user)
        {
            if (string.IsNullOrEmpty(_user.Email) || string.IsNullOrEmpty(_user.MatKhau))
            {
                ViewBag.ErrorInfo = "Vui lòng nhập đầy đủ email và mật khẩu";
                return View("Index");
            }

            var checkaccount = truyen.NguoiDungs.FirstOrDefault(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau);
            if (checkaccount == null)
            {
                ViewBag.ErrorInfo = "Email hoặc mật khẩu không chính xác";
                return View("Index");
            }
            else
            {
                truyen.Configuration.ValidateOnSaveEnabled = false;
                Session["name"] = checkaccount.HoTen;
                Session["id"] = checkaccount.IDuser;
                if (_user.Email == "chiminh521@gmail.com" && _user.MatKhau == "27112004")
                {
                    return RedirectToAction("Index", "truyen");
                }
                else
                {
                    return RedirectToAction("Indexuser", "truyen");
                }
            }
        }
        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        public ActionResult Resgin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Resgin(NguoiDung user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.MatKhau))
            {
                ViewBag.ErrorResgin = "Vui lòng nhập đầy đủ email và mật khẩu";
                return View();
            }

            if (ModelState.IsValid)
            {
                var checkid = truyen.NguoiDungs.FirstOrDefault(f => f.IDuser == user.IDuser);
                if (checkid == null)
                {
                    user.IDuser = Guid.NewGuid().ToString("N").Substring(0, 8);
                    user.NgayTao = DateTime.Now.Date;
                    truyen.NguoiDungs.Add(user);
                    truyen.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorResgin = "Tài khoản đã tồn tại";
                    return View();
                }
            }
            return View();
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string SoDienThoai)
        {
            if (string.IsNullOrEmpty(SoDienThoai))
            {
                ViewBag.Error = "Vui lòng nhập Email.";
                return View();
            }

            var matchingAccounts = truyen.NguoiDungs.Where(e => e.Email == SoDienThoai).ToList();

            if (matchingAccounts.Count == 0)
            {
                ViewBag.Error = "Không tìm thấy tài khoản này.";
                return View();
            }

            var user = matchingAccounts.FirstOrDefault();
            var email = user.Email; // Lấy email của người dùng
            var resetLink = Url.Action("UpdatePassword", "User", new { email = email }, Request.Url.Scheme);
            var mailService = new MailService();
            Task.Run(() => mailService.SendMailAsync(email, "Cập Nhật Mật Khẩu", $"Nhấn <a href='{resetLink}'>vào đây</a> để cập nhật mật khẩu của bạn."));

            ViewBag.Success = "Liên kết cập nhật mật khẩu đã được gửi đến email của bạn.";
            return View();
        }

        // GET: Cập Nhật Mật Khẩu
        public ActionResult UpdatePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return HttpNotFound();
            }

            var user = truyen.NguoiDungs.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View();
            }

            ViewBag.Email = email; // Lưu email vào ViewBag để truyền vào form
            return View(user);
        }

        [HttpPost]
        public ActionResult UpdatePassword(string email, string newPassword, string confirmPassword)
        {
            // Kiểm tra điều kiện hợp lệ
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                ViewBag.Email = email; // Trả lại email vào form
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu không khớp. Vui lòng nhập lại.";
                ViewBag.Email = email; // Trả lại email vào form
                return View();
            }

            // Tìm người dùng bằng email
            var user = truyen.NguoiDungs.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View();
            }

            // Cập nhật mật khẩu
            user.MatKhau = newPassword; // Mã hóa mật khẩu trước khi lưu nếu cần
            truyen.SaveChanges();

            ViewBag.Success = "Cập nhật mật khẩu thành công.";
            return RedirectToAction("Index"); // Chuyển hướng về trang khác nếu cần
        }

    }
}