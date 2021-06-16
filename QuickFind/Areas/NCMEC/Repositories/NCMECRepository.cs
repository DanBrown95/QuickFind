using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using QuickFind.Areas.NCMEC.Models;
using Newtonsoft.Json;

namespace QuickFind.Areas.NCMEC.Repositories
{
    public class NCMECRepository
    {
        public async Task<List<NCMECChild>> PopulateAsync(string state_abbrev)
        {

            APIRoot root = new APIRoot();
            string baseUrl = "https://api.missingkids.org/missingkids/servlet/JSONDataServlet?action=publicSearch&searchLang=en_US&search=new&LanguageId=en_US&caseType=All&firstName=&lastName=&orderBy=MostRecent&subjToSearch=child&missCity=&missState="+state_abbrev+"&missCountry=All&fromDate=&toDate=&age_1=&age_2=&ageMissing_1=&ageMissing_2=&race=All&hairColor=All&eyeColor=All";

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(baseUrl))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    root = JsonConvert.DeserializeObject<APIRoot>(apiResponse);
                }
            }

            return root.Persons;
        }
    }
}