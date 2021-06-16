using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using QuickFind.Areas.FBI.Models;

namespace QuickFind.Areas.FBI.Repositories
{
    public class FBIRepository
    {
        public async Task<List<Person>> PopulateAsync()
        {
            var pageNum = 1;
            var finalPage = 1;
            List<Person> childList = new List<Person>();
            string baseUrl = "https://www.fbi.gov/wanted/kidnap/@@castle.cms.querylisting/querylisting-1?page=";

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
                    if (pageNum == 1)
                    {
                        await page.GoToAsync(baseUrl + pageNum);
                        var finalPageString = await page.QuerySelectorAsync("#query-results-querylisting-1 > div > div > p").EvaluateFunctionAsync<string>("element => element.outerText");
                        finalPage = (int)Math.Ceiling(double.Parse(finalPageString.Split(' ')[1]) / 40);
                    }

                    page = await browser.NewPageAsync();
                    await page.GoToAsync(baseUrl + pageNum);
                    await page.AddScriptTagAsync(new AddTagOptions() { Url = "https://code.jquery.com/jquery-3.2.1.min.js" });
                    var raw = await page.QuerySelectorAllHandleAsync("ul.full-grid li.portal-type-person").EvaluateFunctionAsync<Person[]>("elements => elements.map(a => { return {profile: a.childNodes[1].href, name: a.childNodes[3].innerText}} )");
                    childList.AddRange(raw);

                    pageNum++;
                } while (pageNum <= finalPage);

                foreach (var child in childList)
                {
                    page = await browser.NewPageAsync();
                    await page.GoToAsync(child.Profile);

                    try
                    {
                        var rows = await page.QuerySelectorAllHandleAsync("table.wanted-person-description > tbody > tr > td").EvaluateFunctionAsync<string[]>("elements => elements.map(a => a.innerText)");
                        for (int i = 0; i < rows.Length; i = i+2)
                        {
                            switch (rows[i])
                            {
                                case "Date(s) of Birth Used":
                                    string[] dates = Regex.Matches(rows[i+1], "([^,]*,[^,]*)(?:, |$)").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                                    if(dates.Length == 1)
                                    {
                                        child.BirthDates.Add(new DOB() { Name = child.Name, DateOfBirth = DateTime.Parse(dates[0]) } );
                                    }else if(dates.Length > 1)
                                    {
                                        foreach (var record in dates)
                                        {
                                            var partitions = record.Split(": ");
                                            var date = new DateTime();
                                            var name = "";
                                            if (partitions.Length > 1)
                                            {
                                                name = partitions[0];
                                                DateTime.TryParse((partitions[1]), out date);
                                            }
                                            else
                                            {
                                                DateTime.TryParse((partitions[0]), out date);
                                            }
                                            child.BirthDates.Add(new DOB() { Name = name, DateOfBirth = date });
                                        }
                                    }
                                    break;
                                case "Place of Birth":
                                    child.PlaceOfBirth = rows[i+1];
                                    break;
                                case "Hair":
                                    child.Hair = rows[i+1];
                                    break;
                                case "Eyes":
                                    child.Eyes = rows[i+1];
                                    break;
                                case "Height":
                                    child.Height = rows[i+1];
                                    break;
                                case "Weight":
                                    child.Weight = rows[i+1];
                                    break;
                                case "Sex":
                                    child.Sex = rows[i+1];
                                    break;
                                case "Race":
                                    child.Race = rows[i+1];
                                    break;
                                case "Nationality":
                                    child.Nationality = rows[i+1];
                                    break;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    try
                    {
                        var lastknown = await page.QuerySelectorAsync("div.wanted-person-wrapper > p.summary").EvaluateFunctionAsync<string>("element => element.innerText");
                        var temp = lastknown.Split('\n');
                        if (temp.Count() > 1)
                        {
                            child.LastLocation = lastknown.Split('\n')[1];
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    try
                    {
                        var details = await page.QuerySelectorAsync("div.wanted-person-details > p").EvaluateFunctionAsync<string>("element => element.innerText");
                        child.Details = details;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    await page.CloseAsync();
                }

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