using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace SoWhenExactly.Utils
{
    public static class Aws
    {
        public static AmazonS3Client CreateClient()
        {
            var awsAccessKey = Environment.GetEnvironmentVariable("ITME_AWS_ACCESS_KEY_ID");
            var awsSecret = Environment.GetEnvironmentVariable("ITME_AWS_SECRET_ACCESS_KEY");
            var client = new AmazonS3Client(new BasicAWSCredentials(awsAccessKey, awsSecret), RegionEndpoint.USWest2);
            return client;
        }
    }
}