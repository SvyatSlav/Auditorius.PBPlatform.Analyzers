# Auditorius.PBPlatform.Analyzers

Code Anazyers and Fix'es

To add these analyzers to your project use the NuGet package. In the Visual Studio Package Manager Console exeute the following:
`Install-Package Auditorius.PBPlatform.Analyzers`
  
## Design Analyzers ##
#### [DomainDateTimeUtcNow](https://github.com/SvyatSlav/Auditorius.PBPlatform.Analyzers/blob/master/Auditorius.PBPlatform.Analyzers/Auditorius.PBPlatform.Analyzers/Rules/DomainDateTimeUtcNowAnalyzer.cs) ####
This warning ensures you use in Domain-Layer [DateTime.UtcNow](https://msdn.microsoft.com/library/system.datetime.utcnow(v=vs.110).aspx) neither [DateTime.Now]
