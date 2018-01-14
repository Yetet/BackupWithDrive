using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackupWithDrive.Test
{
  [TestFixture] //This is the collection of tests that will run
  public class TestNewFileDetection
  {
    private string _path;

    [OneTimeSetUp]
    public void FixtureSetup()
    {
      File.Create(_path);

    }

    [Test]
    public void TestIncludeNew()
    {
      // TODO: Add your test code here
      Assert.Pass("Your first passing test");
    }

    [Test]
    public void TestExcludeOld()
    {
      
    }

    [OneTimeTearDown]
    public void FixtureTearDown()
    {
      File.Delete(_path);
    }
  }

}
