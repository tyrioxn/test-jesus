using NUnit.Framework.Interfaces;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.TestRunner;

[assembly:TestRunCallback(typeof(ResultSerializer))]
public class ResultSerializer : ITestRunCallback
{
    public void RunStarted(ITest testsToRun) { }
    public void TestFinished(ITestResult result) { }
    public void TestStarted(ITest test) { }

    public void RunFinished(ITestResult testResults)
    {
        var path = Path.Combine(Application.persistentDataPath, "testresults.xml");
        using (var xmlWriter = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
            testResults.ToXml(true).WriteTo(xmlWriter);

        System.Console.WriteLine($"\n Test results written to: {path}\n");
        Application.Quit(testResults.FailCount > 0 ? 1 : 0);
    }
}