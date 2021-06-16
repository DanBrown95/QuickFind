using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickFind.Areas.IowaOnline.Models;
using PuppeteerSharp;

namespace QuickFind.Areas.IowaOnline.Repositories
{
    public class IowaOnlineRepository
    {
        public async Task<List<IowaOnlineChild>> PopulateAsync()
        {
            var pageNum = 1;
            var finalPage = 1;
            List<IowaOnlineChild> childList = new List<IowaOnlineChild>();
            string baseUrl = "https://www.iowaonline.state.ia.us/mpic/Controller.aspx?cmd=fullReportCommand&curPage=";
            string detailsBaseUrl = "https://www.iowaonline.state.ia.us/mpic/Controller.aspx?cmd=personDetailCommand&id=";

            try
            {
                var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
                {
                    Path = "C:\\Chromium"
                });

                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = "C:\\Users\\dan\\AppData\\Local\\Google\\Chrome\\Application\\chrome.exe"
                });

                var page = await browser.NewPageAsync();

                do
                {
                    if(pageNum == 1)
                    {
                        try
                        {
                            await page.GoToAsync(baseUrl + pageNum);
                            var finalPageString = await page.QuerySelectorAsync("#pageInfo").EvaluateFunctionAsync<string>("element => element.innerText");
                            await page.CloseAsync();
                            finalPage = int.Parse(finalPageString.Split('/')[1]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error retrieving page count", ex);
                        }
                        
                    }

                    try
                    {
                        page = await browser.NewPageAsync();
                        await page.GoToAsync(baseUrl + pageNum);
                        var result = await page.QuerySelectorAllAsync("#resultsTBL tbody tr.rowa, #resultsTBL tbody tr.rowb");
                        await page.CloseAsync();

                        foreach (var row in result)
                        {
                            var contents = (await (await row.GetPropertyAsync("outerText")).JsonValueAsync()).ToString();
                            var detailsId = (await (await row.EvaluateFunctionHandleAsync("element => element.attributes.onclick.value")).JsonValueAsync()).ToString().Split("&id=")[1].Split("';")[0];

                            var splitRow = contents.Split("\t");
                            IowaOnlineChild child = new IowaOnlineChild();
                            child.Name = splitRow[0];
                            child.Sex = splitRow[1];
                            child.LastContact = splitRow[2];
                            child.AgeThen = splitRow[3];
                            child.AgeNow = splitRow[4];
                            child.OriginatingAgency = splitRow[5];
                            child.IncidentType = splitRow[6];

                            //now go to their details page and retrieve the description
                            try
                            {
                                page = await browser.NewPageAsync();
                                await page.GoToAsync(detailsBaseUrl + detailsId);
                                var description = await page.QuerySelectorAsync("#detail_ctl00_descShow").EvaluateFunctionAsync<string>("element => element.innerText");
                                child.Description = description;
                                await page.CloseAsync();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error populating description for: " + child.Name, ex);
                                await page.CloseAsync();
                            }

                            childList.Add(child);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error populating person information", ex);
                        await page.CloseAsync();
                    }
                    
                    pageNum++;
                } while (pageNum <= finalPage);
                   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return childList;
        }
        
    }
}