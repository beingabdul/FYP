using HostalManagement.Models;
using HostalManagement.Models.viewmodels;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HostalManagement.Helpers
{
    public class FaceDetectionHelper
    {
        private static readonly HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        private static readonly IFaceClient faceClient = new FaceClient(
           new ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["subscriptionKey"]),
           new System.Net.Http.DelegatingHandler[] { });
        private static readonly string faceEndpoint = ConfigurationManager.AppSettings["Endpoint"];
        public async Task<Guid?> UploadAndDetectFaces(string imageFilePath)
        {
            // The list of Face attributes to return.
            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
                };

            // Call the Face API.
            try
            {

                if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
                {
                    faceClient.Endpoint = faceEndpoint;


                    using (Stream imageFileStream = File.OpenRead(imageFilePath))
                    {
                        // The second argument specifies to return the faceId, while
                        // the third argument specifies not to return face landmarks.
                        IList<DetectedFace> faceList =
                            await faceClient.Face.DetectWithStreamAsync(
                                imageFileStream, true, false, faceAttributes, RecognitionModel.Recognition04);
                        if (faceList.Count() > 0)
                        {
                            return faceList.First().FaceId;
                        }
                        return new Guid();
                    }
                }
                else
                {
                    return null;
                    throw new InvalidOperationException();
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                //return new DetectedFace();
                return null;
                throw f;
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error");
                return null;
                throw e;
            }
        }

        public async Task<FaceDetectionaVM> ValidateUser(Guid CurrentUserId, int ch_st)
        {
            try
            {
                if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
                {
                    faceClient.Endpoint = faceEndpoint;

                    FaceDetectionaVM Result = new FaceDetectionaVM();

                    var UsersList = db.Registrations.Where(r => r.FaceId != null);
                    foreach (var user in UsersList)
                    {
                        Result.Result = await faceClient.Face.VerifyFaceToFaceAsync((Guid)user.FaceId, CurrentUserId);

                        if (Result.Result.IsIdentical)
                        {

                            Attendance Attandance = new Attendance();
                            Attandance.StdID = user.RegistrationId;
                            if (ch_st == 0)
                            {

                                Attandance.Status = false;
                                Attandance.CheckIn = DateTime.Now;
                                Attandance.Date = DateTime.Now;
                                db.Attendances.Add(Attandance);

                            }
                            else
                            {
                                var us = db.Attendances.Where(st => st.StdID == user.RegistrationId).OrderByDescending(u => u.Date).FirstOrDefault();
                                us.Status = true;
                                us.CheckOut = DateTime.Now;
                                us.Date = DateTime.Now;
                                db.Entry(Attandance).State = EntityState.Modified;
                            }
                            Result.User = user;
                            break;
                        }
                    }
                    return Result;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}