﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using System.Drawing;
using System.Net;
using System.IO;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using CoreMvcWeb.Services.BatchJob;
using System.Net.Mime;
using CoreLib.Http;
using CoreMvcWeb.Models.Lab;
using System.Net.Http;
using StackExchange.Redis;

namespace CoreMvcWeb.Controllers
{
    /// <summary>
    /// 실험실
    /// </summary>
    public class LabController : Controller
    {
        public LabController()
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 바코드 제네레이터
        /// </summary>
        /// <param name="barcode_number"></param>
        /// <returns></returns>
        public IActionResult BarCodeGenerator()
        {
            return View();
        }

        /*
1. yum install -y epel-release
2. yum whatprovides libgdiplus
3. yum install -y libgdiplus-2.10-10.el7.x86_64
4. ln -s /usr/lib/libdl.so.2 /usr/lib/libdl.so
             */
        public IActionResult GetBarcodeImage(string barcodeNumber, string imageType)
        {
            if (barcodeNumber.IsNull() == false)
            {
                try
                {
                    var barcode = new NetBarcode.Barcode(barcodeNumber, NetBarcode.Type.EAN13, true);

                    if (imageType.ToLower() == "base64")
                    {
                        return Content($"data:image/png;base64, {barcode.GetBase64Image()}");
                    }
                    else //png
                    {
                        return File(barcode.GetByteArray(), "image/png", $"barcode_{barcodeNumber}.png");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return Content("");
        }

        public IActionResult DeliveryCheck()
        {
            return View();
        }

        public IActionResult GetDeliveryCheckList(string company_code, string invoice_no)
        {
            //selenium 및 크롬을 통한 웹 크롤링

            /* linux 동작시
             * 1. 구글 크롬 설치
             * centos 7 의 경우
             * 1) repo 등록
vi /etc/yum.repos.d/google-chrome.repo

[google-chrome]
name=google-chrome
baseurl=http://dl.google.com/linux/chrome/rpm/stable/x86_64
enabled=1
gpgcheck=1
gpgkey=https://dl-ssl.google.com/linux/linux_signing_key.pub

             * 2) yum install google-chrome-stable
             * yum install http://dev.naver.com/frs/download.php/443/ttf-nanum-coding-2.0-2.noarch.rpm
             * 2. 구글 크롬 드라이버 설치 // 설치된 크롬 버전에 맞는 드라이버 필요
             * 3. 폰트 설치 // 나눔고딕
cd /usr/share/fonts/
mkdir ./nanumfont
cd ./nanumfont
wget http://cdn.naver.com/naver/NanumFont/fontfiles/NanumFont_TTF_ALL.zip
unzip NanumFont_TTF_ALL.zip

             * 4. webdriver에 실행권한 줄것 chmod 777 과 같이..
             * https://chromedriver.chromium.org/downloads
             */
            var msg = string.Empty;
            var list = new List<DeliveryInfoModel>();

            if (company_code == "EMS")
            {
                var options = new ChromeOptions();
                options.AddArgument("no-sandbox");
                options.AddArgument("headless");
                options.AddArgument("window-size=1920,1080");
                options.AddArgument("disable-gpu");
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
                options.AddArgument("lang=ko-kr");

                using (var driver = new ChromeDriver(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "webdriver"), options))
                {
                    try
                    {
                        IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

                        driver.Url = $"https://service.epost.go.kr/trace.RetrieveEmsRigiTraceList.comm?POST_CODE={invoice_no}";
                        driver.Navigate();

                        wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                        var rows = driver.FindElementsByCssSelector(".detail_off tr");

                        foreach (var row in rows)
                        {
                            var cols = row.FindElements(By.CssSelector("td"));

                            if (cols.Count() < 4 || cols[0].Text.IsNull())
                                continue;

                            var model = new DeliveryInfoModel();

                            model.COMPANY_TYPE = company_code;
                            model.INVOICE_NO = invoice_no;
                            model.UPDATE_DATE = cols[0].Text;
                            model.MOVE_TYPE = cols[1].Text;
                            model.REMARK1 = $"{cols[2].Text} {cols[3].Text}";
                            list.Add(model);
                        }

                        msg = "OK";
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
            }

            return Json(new { msg = msg, list = list });
        }

        public IActionResult WebFileDownload()
        {
            return View();
        }

        public async Task<IActionResult> UploadTest()
        {
            var files = Request.Form.Files.GetFiles("file1");

            foreach(var file in files)
            {
                using (var fs = new FileStream($"d:/tmp/{file.FileName}", FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
            }

            return Json(new { msg = "OK" });
        }

        public async Task<IActionResult> GetWebFileDownload(string url)
        {
            try
            {
                using (var client = new HttpClientHelper())
                {
                    /* Download */
                    var file = await client.GetFileAsync(HttpMethod.Get, url);
                    file.SaveAs($"DownloadFiles/{file.FileName}", true, (now) => {
                        Console.WriteLine($"download {file.FileName} : {file.TotalBytesSize} / {now}");
                    });

                    /* Upload */
                    /*
                    var fileList = new List<HttpFile>();
                    fileList.Add(new HttpFile("file1", "d:/Docker for Windows Installer.exe"));
                    fileList.Add(new HttpFile("file1", "d:/VMware-VMvisor-Installer-6.7.0.update02-13006603.x86_64.iso"));

                    await client.SendMultipartAsync(HttpMethod.Post, "http://localhost:5000/lab/uploadtest", null, fileList, (now, tot) =>
                    {
                        Console.WriteLine($"total upload: {now} / {tot}");
                    },
                    (fileName, now, tot) =>
                    {
                        Console.WriteLine($"{fileName} upload: {now} / {tot}");
                    });
                    */
                }

                return Json(new { msg = "OK", fileName = "test" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }


        public IActionResult DbTableList()
        {
            return View();
        }

        public IActionResult GetDbInfo(string tableName)
        {
            var schemaName = "home";

            if (tableName.IsNull())
            {
                return Json(MySqlDbTableModel.GetTableList(schemaName));
            }

            var list = MySqlDbTableModel.GetTableColumnsList(schemaName, tableName);
            var modelClassName = $"{tableName.Substring(0, 1).ToUpper()}{tableName.Substring(1)}Model";

            return Json(new
            {
                list = list,
                modelString = MySqlDbTableModel.GetClassModel(list, modelClassName),
                sqlString = MySqlDbTableModel.GetXmlSqlString(list, tableName)
            });
        }

        public IActionResult RedisTest()
        {
            var connectionString = "192.168.0.200:6379";

            var redisConnection = ConnectionMultiplexer.Connect(connectionString);
            if (redisConnection.IsConnected)
            {
                var db = redisConnection.GetDatabase();
                db.HashSet("hash1", new [] {
                    new HashEntry("key1", "val1"),
                    new HashEntry("key2", "val2"),
                }); 

                var val = db.HashGet("hash1", "key1");
                if (val.IsNullOrEmpty)
                {
                    
                }
                db.StringSet("str1", "val11");

                var val2 = db.StringGet("str1");
                
            }

            return View();
        }

    }
}
