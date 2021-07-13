using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostalManagement.Models.viewmodels
{
    public class FaceDetectionaVM
    {
        public VerifyResult Result { get; set; }
        public Registration User { get; set; }

    }
}