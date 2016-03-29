using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Bellona.Analysis")]
[assembly: AssemblyDescription("The library for statistical analysis.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Keiho Sakapon")]
[assembly: AssemblyProduct("Bellona")]
[assembly: AssemblyCopyright("© 2015 Keiho Sakapon")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/sakapon/Bellona.Analysis")]
[assembly: AssemblyMetadata("LicenseUrl", "https://github.com/sakapon/Bellona.Analysis/blob/master/LICENSE")]
[assembly: AssemblyMetadata("Tags", "Machine Learning Clustering")]
[assembly: AssemblyMetadata("ReleaseNotes", "The first stable release that has the features for clustering.")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("6134e5f8-5f37-4568-b2cb-49fed28cad2c")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.11.0")]
[assembly: AssemblyFileVersion("1.0.11")]

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("UnitTest")]
