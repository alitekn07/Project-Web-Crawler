USE [master]
GO
/****** Object:  Database [WebCrawlerProject]    Script Date: 23/01/2023 02:45:51 ******/
CREATE DATABASE [WebCrawlerProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WebCrawlerProject', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\WebCrawlerProject.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'WebCrawlerProject_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\WebCrawlerProject_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [WebCrawlerProject] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WebCrawlerProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WebCrawlerProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WebCrawlerProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WebCrawlerProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET  DISABLE_BROKER 
GO
ALTER DATABASE [WebCrawlerProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WebCrawlerProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET RECOVERY FULL 
GO
ALTER DATABASE [WebCrawlerProject] SET  MULTI_USER 
GO
ALTER DATABASE [WebCrawlerProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WebCrawlerProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WebCrawlerProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WebCrawlerProject] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [WebCrawlerProject] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [WebCrawlerProject] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [WebCrawlerProject] SET QUERY_STORE = OFF
GO
USE [WebCrawlerProject]
GO
/****** Object:  Table [dbo].[tblMainUrls]    Script Date: 23/01/2023 02:45:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMainUrls](
	[UrlHash] [char](64) NOT NULL,
	[Url] [nvarchar](200) NOT NULL,
	[DiscoverDate] [datetime] NOT NULL,
	[LinkDepthLevel] [smallint] NOT NULL,
	[ParentUrlHash] [char](64) NOT NULL,
	[LastCrawlingDate] [datetime] NOT NULL,
	[SourceCode] [varchar](max) NULL,
	[FetchTimeMS] [int] NOT NULL,
	[PageTitle] [nvarchar](max) NULL,
	[CompressionPercent] [tinyint] NOT NULL,
	[IsCrawled] [bit] NOT NULL,
	[HostUrl] [nvarchar](200) NOT NULL,
	[CrawlTryCounter] [tinyint] NOT NULL,
 CONSTRAINT [PK_tblMainUrls] PRIMARY KEY CLUSTERED 
(
	[UrlHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [select_query_tuning_index]    Script Date: 23/01/2023 02:45:51 ******/
CREATE NONCLUSTERED INDEX [select_query_tuning_index] ON [dbo].[tblMainUrls]
(
	[IsCrawled] ASC,
	[CrawlTryCounter] DESC,
	[DiscoverDate] ASC
)
INCLUDE([Url],[LinkDepthLevel]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblMainUrls] ADD  CONSTRAINT [DF_tblMainUrls_DiscoverDate]  DEFAULT (sysdatetime()) FOR [DiscoverDate]
GO
ALTER TABLE [dbo].[tblMainUrls] ADD  CONSTRAINT [DF_tblMainUrls_LinkDepthLevel]  DEFAULT ((0)) FOR [LinkDepthLevel]
GO
ALTER TABLE [dbo].[tblMainUrls] ADD  CONSTRAINT [DF_tblMainUrls_FetchTimeMS]  DEFAULT ((0)) FOR [FetchTimeMS]
GO
ALTER TABLE [dbo].[tblMainUrls] ADD  CONSTRAINT [DF_tblMainUrls_CompressionPercent]  DEFAULT ((100)) FOR [CompressionPercent]
GO
ALTER TABLE [dbo].[tblMainUrls] ADD  CONSTRAINT [DF_tblMainUrls_IsCrawled]  DEFAULT ((0)) FOR [IsCrawled]
GO
USE [master]
GO
ALTER DATABASE [WebCrawlerProject] SET  READ_WRITE 
GO
