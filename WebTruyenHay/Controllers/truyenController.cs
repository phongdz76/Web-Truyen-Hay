using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    public class truyenController : Controller
    {
        truyenhayEntities7 truyendata = new truyenhayEntities7();
        // GET: truyen
        public async Task<ActionResult> Index(string category)
        {
            await NotifyNewChapters();
            if (category == null)
            {
                var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe);
                return View(productList);
            }
            else
            {
                var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe)
                .Where(x => x.theloai == category);
                return View(productList);
            }
        }
        public async Task<ActionResult> Indexuser(string category)
        {
            await NotifyNewChapters();
            if (category == null)
            {
                var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe);
                return View(productList);
            }
            else
            {
                var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe)
                .Where(x => x.theloai == category);
                return View(productList);
            }

        }
        // tạo truyện
        public ActionResult Create()
        {
            Truyen truyen1 = new Truyen();
            return View(truyen1);
        }
        [HttpPost]

        public ActionResult Create(Truyen truyen1)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    if (truyen1.UploadImage != null && truyen1.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(truyen1.UploadImage.FileName);
                        string extent = Path.GetExtension(truyen1.UploadImage.FileName);
                        filename = filename + extent;
                        truyen1.imagetruyen = "~/Content/images/" + filename;
                        truyen1.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                    }
                   
                    truyen1.IDtruyen = Guid.NewGuid().ToString("N").Substring(0, 8);
                    truyen1.NgayTao = DateTime.Now;
                    truyendata.Truyens.Add(truyen1);
                    truyendata.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; 
            }
            return View(truyen1);
        }
        // edit truyện
        public ActionResult Edit(String id)
        {
            var product = truyendata.Truyens.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Truyen sach)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = truyendata.Truyens.Find(sach.IDtruyen);
                    if (existingProduct == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin truyên
                    existingProduct.TieuDe = sach.TieuDe;
                    existingProduct.MoTa = sach.MoTa;
                    existingProduct.theloai = sach.theloai;
                    existingProduct.TrangThai = sach.TrangThai;

                    // Cập nhật hình ảnh nếu có
                    if (sach.UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(sach.UploadImage.FileName);
                        string extent = Path.GetExtension(sach.UploadImage.FileName);
                        filename = filename + extent;
                        existingProduct.imagetruyen = "~/Content/images/" + filename;
                        sach.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                    }

                    // Lưu thay đổi vào database
                    truyendata.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(sach);
            }
            catch
            {
                return View();
            }
        }
        //xem chi tiết của người dungf
        public ActionResult Detailsuser(String id)
        {
            var truyen = truyendata.Truyens.Find(id);
            if (truyen == null)
            {
                return HttpNotFound();
            }

            var chapters = truyendata.Chuongs.Where(c => c.TruyenID == id).ToList();

            var viewModel = new TruyenWithChapters
            {
                Truyen = truyen,
                Chapters = chapters
            };

            return View(viewModel);
        }
        public ActionResult Details(String id)
        {
            var truyen = truyendata.Truyens.Find(id);
            if (truyen == null)
            {
                return HttpNotFound();
            }
            var ViewChapters = truyendata.Truyens.Where(c => c.IDtruyen == id).FirstOrDefault();
            var chapters = truyendata.Chuongs.Where(c => c.TruyenID == id).OrderByDescending(c => c.SoThuTu).ToList();
            var danhgia1 = truyendata.DanhGias.Where(s => s.TruyenID == id).ToList();
            ViewChapters.viewtruyen += 1;
            truyendata.SaveChanges(); 

            var viewModel = new TruyenWithChapters
            {
                Truyen = truyen,
                Chapters = chapters,
                DanhGia = danhgia1
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //add bình luận
        public ActionResult AddComment(string id, TruyenWithChapters model)
        {
            if (ModelState.IsValid)
            {
                var newComment = new DanhGia
                {
                    IDDanhGia = Guid.NewGuid().ToString("N").Substring(0, 8),
                    TruyenID = id,
                    NhanXet = model.NewComment,
                    DiemDanhGia = model.Rating,
                    NgayDanhGia = DateTime.Now
                };

                truyendata.DanhGias.Add(newComment);
                truyendata.SaveChanges();

                return RedirectToAction("Details", new { id = id });
            }

        
            return RedirectToAction("Details", new { id = id });
        }
        // xóa
        public ActionResult Delete(String id)
        {
            var sach = truyendata.Truyens.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            // Xóa sản phẩm
            truyendata.Truyens.Remove(sach);
            truyendata.SaveChanges();

            return RedirectToAction("Index");
        }
        //tìm kiếm
        public ActionResult Search(string searchString)
        {
            var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe);

            if (!string.IsNullOrEmpty(searchString))
            {
                productList = (IOrderedQueryable<Truyen>)productList.Where(x => x.TieuDe.Contains(searchString));
            }

            return View("Index", productList.ToList());
        }
        public ActionResult Searchuser(string searchString)
        {
            var productList = truyendata.Truyens.OrderByDescending(x => x.TieuDe);

            if (!string.IsNullOrEmpty(searchString))
            {
                productList = (IOrderedQueryable<Truyen>)productList.Where(x => x.TieuDe.Contains(searchString));
            }
            var email = "minhchi521@gmail.com"; // Replace with the actual email or retrieve it dynamically
            var mailService = new MailService();
            Task.Run(() => mailService.SendMailAsync(email, "Test new", "Sản phẩm mượn bị quá hạn"));
            return View("Indexuser", productList.ToList());
        }
        // tìm kiếm nâng cao
        public ActionResult AdvancedSearch(string title, string author, string category)
        {
            var books = truyendata.Truyens.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                books = books.Where(b => b.TieuDe.Contains(title));
            }

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.TacGia.Contains(author));
            }

            if (!string.IsNullOrEmpty(category))
            {
                books = books.Where(b => b.theloai.Contains(category));
            }
            return View("Index", books.ToList());
        }
        public ActionResult AdvancedSearchuser(string title, string author, string category)
        {
            var books = truyendata.Truyens.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                books = books.Where(b => b.TieuDe.Contains(title));
            }

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.TacGia.Contains(author));
            }

            if (!string.IsNullOrEmpty(category))
            {
                books = books.Where(b => b.theloai.Contains(category));
            }
            return View("Indexuser", books.ToList());
        }
        //tạo chương cho truyện
        public ActionResult chuong(String id)
        {
            var sach = truyendata.Truyens.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }

            var chuongMoi = new Chuong
            {
                TruyenID = id.ToString()
            };

            return View(chuongMoi);
        }

        [HttpPost]
        public ActionResult chuong(Chuong chuong,String id)
        {
            if (ModelState.IsValid)
            {
                chuong.TruyenID = id;
                chuong.IDchuong = Guid.NewGuid().ToString("N").Substring(0, 8);
                chuong.NgayDang = DateTime.Now;
                truyendata.Chuongs.Add(chuong);
                truyendata.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }

            return View(chuong);
        }
        // đọc truyện
        public ActionResult ReadTruyen(string id, int? thutu)
        {
            Chuong checkid;
            if (!string.IsNullOrEmpty(id))
            {
                checkid = truyendata.Chuongs.FirstOrDefault(s => s.IDchuong == id);
            }
            else if (thutu.HasValue)
            {
                checkid = truyendata.Chuongs.FirstOrDefault(s => s.SoThuTu == thutu.Value);
            }
            else
            {
                return HttpNotFound();
            }

            if (checkid == null)
            {
                return HttpNotFound();
            }

            var image = truyendata.imagechuongs.Where(s => s.IDchuong == checkid.IDchuong).OrderBy(s => s.sothutu).ToList();
            var viewModel = new ChuongAndImage
            {
                chuong = checkid,
                Chapters = image
            };
            var checktiendo = truyendata.TienDoDocs.Where(S => S.cacchuongdadoc == id).FirstOrDefault();
            if(checktiendo == null)
            {
                var tiendodoc = new TienDoDoc
                {
                    idtiendo = Guid.NewGuid().ToString("N").Substring(0, 8),
                    NguoiDungID = GetCurrentUserId(),
                    TruyenID = checkid.TruyenID,
                    cacchuongdadoc = id,
                    ChuongHienTai=id,
                    NgayCapNhat = DateTime.Now.Date
                };
                truyendata.TienDoDocs.Add(tiendodoc);
                truyendata.SaveChanges();
            }
            return View(viewModel);
        }
        //kiẻm tra 
        private bool IsLastChapter(int thutu)
        {
            return !truyendata.Chuongs.Any(c => c.SoThuTu > thutu);
        }
        // chuyển trang

        public ActionResult nextchap(int thutu)
        {
            var user = GetCurrentUserId();
            var currentChapter = truyendata.Chuongs.FirstOrDefault(c => c.SoThuTu == thutu);
            if (currentChapter == null)
            {
                return HttpNotFound();
            }
            var maxChapterNumber = truyendata.Chuongs.Max(s => s.SoThuTu);
            var isLastChapter = thutu >= maxChapterNumber;

            if (!isLastChapter)
            {
                var nextChapter = truyendata.Chuongs
                                  .Where(c => c.SoThuTu > thutu)
                                  .OrderBy(c => c.SoThuTu)
                                  .FirstOrDefault();
                if (nextChapter == null)
                {
                    ModelState.AddModelError("nextchap", "Hết chương rồi.");
                    return View("ReadTruyen", new ChuongAndImage { chuong = currentChapter, IsLastChapter = true });
                }
                var image = truyendata.imagechuongs.Where(s => s.IDchuong == nextChapter.IDchuong).ToList();
                var viewModel = new ChuongAndImage
                {
                    chuong = nextChapter,
                    Chapters = image,
                    IsLastChapter = false
                };
                var tiendodoc = truyendata.TienDoDocs.Where(s => s.NguoiDungID == user && s.TruyenID == currentChapter.TruyenID).FirstOrDefault();
                tiendodoc.ChuongHienTai = nextChapter.IDchuong;
                truyendata.SaveChanges();
                return View("ReadTruyen", viewModel);
            }
            else
            {
                var image = truyendata.imagechuongs.Where(s => s.IDchuong == currentChapter.IDchuong).ToList();
                var viewModel = new ChuongAndImage
                {
                    chuong = currentChapter,
                    Chapters = image,
                    IsLastChapter = true
                };
                return View("ReadTruyen", viewModel);
            }

        }
        // lùi trang
        public ActionResult returnchap(int thutu)
        {
            var currentChapter = truyendata.Chuongs.FirstOrDefault(c => c.SoThuTu == thutu);
            if (currentChapter == null)
            {
                return HttpNotFound();
            }

            var returnChapter = truyendata.Chuongs
                .Where(c => c.SoThuTu < thutu)
                .OrderByDescending(c => c.SoThuTu)
                .FirstOrDefault();

            if (returnChapter == null)
            {
                return RedirectToAction("FirstChapter");
            }

            
            var image = truyendata.imagechuongs.Where(s => s.IDchuong == returnChapter.IDchuong).ToList();
            var viewModel = new ChuongAndImage
            {
                chuong = returnChapter,
                Chapters = image
            };

            return View("ReadTruyen", viewModel);
        }
        // tạo nội dung cho chương truyện
        public ActionResult noidungchap(String id)
        {
            var checkid = truyendata.Chuongs.Find(id);
            if(checkid == null)
            {
                return HttpNotFound();
            }
            var noidung = new imagechuong
            {
                IDchuong = id
            };
            return View(noidung);
        }
        [HttpPost]
        public ActionResult noidungchap(imagechuong chuong)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (chuong.UploadImage != null && chuong.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(chuong.UploadImage.FileName);
                        string extent = Path.GetExtension(chuong.UploadImage.FileName);
                        filename = filename + extent;
                        chuong.imagechuong1 = "~/Content/images/" + filename;
                        chuong.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                    }
                    
                    chuong.IDchuongimage = Guid.NewGuid().ToString("N").Substring(0, 8);
                    int maxSoThuTu = truyendata.imagechuongs.Max(c => (int?)c.sothutu) ?? 0;
                    chuong.sothutu = maxSoThuTu + 1;

                    truyendata.imagechuongs.Add(chuong);
                    truyendata.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(chuong);
        }
        //mua premiu
        public ActionResult BuyPremium()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessBuyPremium()
        {
            try
            {
                string currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    throw new Exception("User not authenticated");
                }

                decimal premiumPrice = 2000;
                string transactionId = Guid.NewGuid().ToString("N").Substring(0, 8);
                var transaction = new Giaodich
                {
                    IDtransaction = transactionId,
                    UserID = currentUserId,
                    TransactionDate = DateTime.Now,
                    Amount = premiumPrice,
                    IsSuccessful = "Pending",
                    TransactionCode = transactionId
                };
                truyendata.Giaodiches.Add(transaction);
               
                var paymentService = new PaymentService();
                string orderInfo = $"Premium Subscription - {transactionId}";
                string redirectUrl = Url.Action("PremiumSuccess", "truyen",new {id = transactionId }, Request.Url.Scheme);
                string callbackUrl = Url.Action("PremiumFailure", "truyen", null, Request.Url.Scheme);
                string paymentUrl = await paymentService.CreateMoMoPaymentAsync(premiumPrice, orderInfo, redirectUrl, callbackUrl);

                if (string.IsNullOrEmpty(paymentUrl))
                {
                    throw new Exception("Failed to create MoMo payment URL");
                }
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (truyendata.Giaodiches.Any(g => g.IsSuccessful == "Pending"))
                {
                    var failedTransaction = truyendata.Giaodiches.First(g => g.IsSuccessful == "Pending");
                    failedTransaction.IsSuccessful = "Failed";
                    await truyendata.SaveChangesAsync();
                }
                ViewBag.ErrorMessage = "An error occurred while processing your payment. Please try again later.";
                return View("BuyPremium");
            }
        }


        [HttpGet]
        public async Task<ActionResult> PaymentCallback(string partnerCode, string orderId, string requestId, string amount, string orderInfo, string orderType, string transId, string resultCode, string message, string payType, string responseTime, string extraData, string signature)
        {
            try
            {
                // Verify the signature here (you'll need to implement this)
                // if (!VerifySignature(signature, ...)) {
                //     throw new Exception("Invalid signature");
                // }

                var transaction = truyendata.Giaodiches
                    .FirstOrDefault(t => t.TransactionCode == orderId);

                if (transaction != null)
                {
                    transaction.IsSuccessful = resultCode == "0" ? "Success" : "Failure";

                    if (transaction.IsSuccessful == "Success")
                    {
                        // Cập nhật trạng thái premium cho user
                        var user = truyendata.NguoiDungs.Find(transaction.UserID);
                        if (user != null)
                        {
                            user.VaiTro = "true"; // Assuming 1 month premium
                        }
                    }

                    await truyendata.SaveChangesAsync();

                    if (transaction.IsSuccessful == "Success")
                    {
                        TempData["SuccessMessage"] = "Thanh toán thành công! Bạn đã trở thành Premium User.";
                        return RedirectToAction("PremiumSuccess");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Thanh toán thất bại. Vui lòng thử lại.";
                        return RedirectToAction("PremiumFailure");
                    }
                }
                return RedirectToAction("Error", new { message = "Không tìm thấy giao dịch" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }
        public ActionResult PremiumSuccess( String id)
        {

            try
            {
                if (id != null)
                {
                    var user1 = GetCurrentUserId();
                    var user = truyendata.NguoiDungs.Find(user1);
                    if (user != null)
                    {
                        user.VaiTro = "true";
                    }

                    // Sửa giá trị Amount phù hợp với kiểu dữ liệu trong DB
                    var transaction = new Giaodich
                    {
                        IDtransaction = id,
                        UserID = user1,
                        TransactionDate = DateTime.Now,
                        Amount = 20, // Giảm giá trị xuống hoặc điều chỉnh theo đúng kiểu dữ liệu
                        IsSuccessful = "Pending",
                        TransactionCode = id
                    };

                    truyendata.Giaodiches.Add(transaction);
                    truyendata.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("PremiumFailure");
                }
               
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return RedirectToAction("Error");
            }
        }

        // GET: truyen/PremiumFailure
        public ActionResult PremiumFailure()
        {
            return View();
        }

        public ActionResult profile()
        {
            var manguoidung = GetCurrentUserId();
            var checkid = truyendata.NguoiDungs.Find(manguoidung);
            if (checkid == null)
            {
                return HttpNotFound();
            }
            return View(checkid);
        }
        [HttpPost]
        public ActionResult profile(NguoiDung user, HttpPostedFileBase UploadImage)
        {
            var manguoidung = GetCurrentUserId();
            var checkid = truyendata.NguoiDungs.FirstOrDefault(s => s.IDuser == manguoidung);

            if (checkid == null)
            {
                return HttpNotFound();
            }


            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                string extension = Path.GetExtension(UploadImage.FileName);
                filename = filename + extension;
                string imagePath = "~/Content/images/" + filename;


                checkid.imageuser = imagePath;


                UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
            }

            checkid.HoTen = user.HoTen;

            truyendata.SaveChanges();

            return RedirectToAction("profile");
        }
        public ActionResult tiendodoc(String sothutu)
        {
            try
            {
                // Tìm tiến độ đọc dựa trên số thứ tự chương (hoặc ID chương)
                var tiendo = truyendata.TienDoDocs
                    .FirstOrDefault(td => td.ChuongHienTai == sothutu);

                if (tiendo == null)
                {
                    return HttpNotFound("Không tìm thấy tiến độ đọc cho số thứ tự này.");
                }

                return View(tiendo);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error: {ex.Message}");
                return View("Error", new { message = ex.Message });
            }
        }
        //chức năng top 
        public ActionResult top10()
        {
            var top10Truyen = truyendata.Truyens
           .OrderByDescending(s => s.viewtruyen) 
           .Take(5) 
           .ToList();
            return PartialView("_Top10Truyen", top10Truyen);
        }
        //chức năng iu thích truyện
        public ActionResult iuthichtruyen(String id)
        {
            var checkid = truyendata.Truyens.Find(id);
            if (checkid == null)
            {
                return HttpNotFound();
            }

            var userId = GetCurrentUserId();
            var existingLike = truyendata.truyeniuthiches.FirstOrDefault(t => t.idsach == id && t.NguoiDungID == userId);

            if (existingLike == null)
            {
                // Thêm like mới
                var iuthich = new truyeniuthich
                {
                    ID = Guid.NewGuid().ToString("N").Substring(0, 8),
                    idsach = id,
                    NguoiDungID = userId
                };
                truyendata.truyeniuthiches.Add(iuthich);
                truyendata.SaveChanges();
                return Json(new { isLiked = true });
            }
            else
            {
                // Xóa like
                truyendata.truyeniuthiches.Remove(existingLike);
                truyendata.SaveChanges();
                return Json(new { isLiked = false });
            }
        }
        //
        public ActionResult ToggleTheme()
        {
            string currentTheme = Session["Theme"] as string ?? "light";
            string newTheme = currentTheme == "light" ? "dark" : "light";
            Session["Theme"] = newTheme;
            return Json(new { success = true, theme = newTheme }, JsonRequestBehavior.AllowGet);
        }
        //xem truyện iu thích
        public ActionResult viewTruyeniuthich()
        {
            var checkid = GetCurrentUserId();
            var truyeniu = truyendata.truyeniuthiches.Where(s => s.NguoiDungID == checkid).ToList();
            var truyeniuthic = from iu in truyendata.truyeniuthiches
                               join sac in truyendata.Truyens on iu.idsach equals sac.IDtruyen
                               join user in truyendata.NguoiDungs on iu.NguoiDungID equals user.IDuser
                               select new TruyenIuModel
                               {
                                   masach=sac.IDtruyen,
                                   hinh=sac.imagetruyen,
                                   manguoidung=user.IDuser
                               };
            var tuyenhay = truyeniuthic.Where(s => s.manguoidung == checkid).ToList();
            return PartialView("viewTruyeniuthich", tuyenhay);
        } 
        //đọc tiếp truyện
        public ActionResult doctiep()
        {
            var user = GetCurrentUserId();

            // Kiểm tra nếu không tìm thấy user
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index", "Home"); 
            }

            var doctiep = truyendata.TienDoDocs
                                   .Where(s => s.NguoiDungID == user)
                                   .OrderByDescending(s => s.sothutu)
                                   .FirstOrDefault();

            // Kiểm tra nếu không có tiến độ đọc
            if (doctiep == null || string.IsNullOrEmpty(doctiep.ChuongHienTai))
            {
                // Redirect về trang chủ hoặc trang truyện
                return RedirectToAction("Index");

            }

            return RedirectToAction("ReadTruyen", new { id = doctiep.ChuongHienTai });
        }
        //đọc chương mới nhất 
        public ActionResult docchuongmoi() 
        {
            var lastChapter = truyendata.Chuongs
                             .OrderByDescending(s => s.SoThuTu)
                             .FirstOrDefault();
            if (lastChapter == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("ReadTruyen", new { id = lastChapter.IDchuong });
        }
        //đọc chuong củ nhất 
        public ActionResult doccunhat()
        {
            var lastChapter = truyendata.Chuongs
                             .OrderBy(s => s.SoThuTu)
                             .FirstOrDefault();
            if (lastChapter == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("ReadTruyen", new { id = lastChapter.IDchuong });
        }
        //thông báo cho người đọc truyện iu thích có chương mới
        public async Task<ActionResult> NotifyNewChapters()
        {
            var userId = GetCurrentUserId();
            var today = DateTime.Now.Date;

            var updatedStories = await (from favorite in truyendata.truyeniuthiches
                                        join truyen in truyendata.Truyens on favorite.idsach equals truyen.IDtruyen
                                        join chuong in truyendata.Chuongs on truyen.IDtruyen equals chuong.TruyenID
                                        where favorite.NguoiDungID == userId
                                        && chuong.NgayDang == today
                                        select new
                                        {
                                            IDtruyen = truyen.IDtruyen,
                                            TieuDe = truyen.TieuDe,
                                            SoChuong = chuong.SoThuTu
                                        }).Distinct().ToListAsync();

            if (!updatedStories.Any())
            {
                return new HttpStatusCodeResult(200);
            }

            var user = GetCurrentUserId();
            if (string.IsNullOrEmpty(user))
            {
                return new HttpStatusCodeResult(400);
            }

            // Tạo thông báo database
            foreach (var story in updatedStories)
            {
                var thongBao = new ThongBao
                {
                    IDtb = Guid.NewGuid().ToString(),
                    NguoiDungID = userId,
                    TruyenID = story.IDtruyen,
                    NoiDung = $"Truyện {story.TieuDe} đã có chương mới: Chương {story.SoChuong}",
                    DaDoc = 0,
                    NgayTao = today
                };
                truyendata.ThongBaos.Add(thongBao);
            }
            await truyendata.SaveChangesAsync();

            // Tạo nội dung email
            string subject = "Thông báo truyện mới";

            // Tạo body message không sử dụng \n
            var messageBuilder = new System.Text.StringBuilder();
            messageBuilder.Append("Các truyện yêu thích của bạn đã cập nhật chương mới: ");

            foreach (var story in updatedStories)
            {
                messageBuilder.Append($" - {story.TieuDe}: Chương {story.SoChuong}. ");
            }

            string body = messageBuilder.ToString();

            var MailService1 = new MailService();
            await MailService1.SendMailAsync(user, subject, body);

            return new HttpStatusCodeResult(200);
        }
        //list truyện đã đọc
        public ActionResult truyendangdoc()
        {
            var checkid = GetCurrentUserId();
            var truyeniu = truyendata.TienDoDocs.Where(s => s.NguoiDungID == checkid).ToList();
            var truyeniuthic = from tiendo in truyendata.TienDoDocs
                               join sac in truyendata.Truyens on tiendo.TruyenID equals sac.IDtruyen
                               join user in truyendata.NguoiDungs on tiendo.NguoiDungID equals user.IDuser
                               select new truyenđocmodel
                               {
                                   masach = sac.IDtruyen,
                                   hinh = sac.imagetruyen,
                                   manguoidung = user.IDuser
                               };
            var tuyenhay = truyeniuthic.Where(s => s.manguoidung == checkid).ToList();
            return PartialView("truyendangdoc", tuyenhay);
        }
        // lấy ID người dùng
        private string GetCurrentUserId()
        {
            return Session["id"] as string ?? "DefaultUserId";
        }
    }
} 