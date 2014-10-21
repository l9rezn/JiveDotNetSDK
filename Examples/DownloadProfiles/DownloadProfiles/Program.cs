﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Net.Pokeshot.JiveSdk.Models;

namespace DownloadProfiles
{
    class Program
    {
      
        static  void Main(string[] args)
        {
            getPeople();

            Console.ReadLine();
        }

        static async void getPeople()
        {
            try
            {
                string jiveCommunityUrl = "https://jiveinstance.jiveon.com";



                string userName = "username";
                string password = "password";


                System.Net.Http.HttpClientHandler jiveHandler = new System.Net.Http.HttpClientHandler();




                NetworkCredential myCredentials = new NetworkCredential(userName, password);
                myCredentials.Domain = jiveCommunityUrl + "/api/core/v3";


                string cre = String.Format("{0}:{1}", userName, password);

                byte[] bytes = Encoding.UTF8.GetBytes(cre);

                string base64 = Convert.ToBase64String(bytes);





                jiveHandler.Credentials = myCredentials;
                jiveHandler.PreAuthenticate = true;
                jiveHandler.UseDefaultCredentials = true;

                System.Net.Http.HttpClient httpClient;

                httpClient = new System.Net.Http.HttpClient(jiveHandler);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);


                System.Net.Http.HttpResponseMessage activityResponse = await httpClient.GetAsync(String.Format(jiveCommunityUrl + "/api/core/v3/people"));

                String myActivityResponse = await activityResponse.Content.ReadAsStringAsync();
                string cleanResponseActivities = myActivityResponse.Replace("throw 'allowIllegalResourceCall is false.';", "");

                //Console.WriteLine(cleanResponseActivities);

                PeopleList myPeople = JsonConvert.DeserializeObject<PeopleList>(cleanResponseActivities);
                foreach (Person myPerson in myPeople.list)
                {
                    System.Net.Http.HttpResponseMessage imageReponse = await httpClient.GetAsync(myPerson.resources.images.@ref);

                    String myImagesReponse = await imageReponse.Content.ReadAsStringAsync();
                    string cleanImageResponse = myImagesReponse.Replace("throw 'allowIllegalResourceCall is false.';", "");
                    ImageList myImageList = JsonConvert.DeserializeObject<ImageList>(cleanImageResponse);
                    foreach (Image myImage in myImageList.list)
                    {
                        Console.WriteLine(myImage.@ref);

                        byte[] data;
                        using (WebClient client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.Authorization] = "Basic " + base64;
                            data = client.DownloadData(myImage.@ref);
                        }
                        System.IO.File.WriteAllBytes(@"c:\images\" + myPerson.jive.username +"_" + myImage.index.ToString() + ".png", data);

                    }

                    
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}