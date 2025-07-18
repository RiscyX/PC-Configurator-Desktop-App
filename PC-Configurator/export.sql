USE [master]
GO
/****** Object:  Database [PCConfiguratorDB]    Script Date: 2025. 07. 11. 0:43:37 ******/
CREATE DATABASE [PCConfiguratorDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PCConfiguratorDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PCConfiguratorDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PCConfiguratorDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PCConfiguratorDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PCConfiguratorDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PCConfiguratorDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PCConfiguratorDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PCConfiguratorDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PCConfiguratorDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PCConfiguratorDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PCConfiguratorDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET RECOVERY FULL 
GO
ALTER DATABASE [PCConfiguratorDB] SET  MULTI_USER 
GO
ALTER DATABASE [PCConfiguratorDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PCConfiguratorDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PCConfiguratorDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PCConfiguratorDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PCConfiguratorDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PCConfiguratorDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'PCConfiguratorDB', N'ON'
GO
ALTER DATABASE [PCConfiguratorDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [PCConfiguratorDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PCConfiguratorDB]
GO
/****** Object:  Table [dbo].[SocketTypes]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocketTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SocketName] [nvarchar](50) NOT NULL,
	[Manufacturer] [nvarchar](50) NOT NULL,
	[Generation] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SocketName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Motherboards]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Motherboards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Manufacturer] [nvarchar](100) NULL,
	[ChipsetTypeId] [int] NOT NULL,
	[SocketTypeId] [int] NOT NULL,
	[MemorySlots] [int] NULL,
	[MaxMemoryGB] [int] NULL,
	[FormFactor] [nvarchar](50) NULL,
	[Price] [int] NULL,
	[PowerConsumption] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[MotherboardView]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Létrehozzuk a MotherboardView nézetet
CREATE VIEW [dbo].[MotherboardView] AS
SELECT 
    m.Id,
    m.Name,
    m.Manufacturer,
    m.ChipsetTypeId,
    m.SocketTypeId,
    s.SocketName,
    s.Manufacturer AS SocketManufacturer,
    m.MemorySlots,
    m.MaxMemoryGB,
    m.FormFactor,
    m.Price
FROM 
    Motherboards m
LEFT JOIN 
    SocketTypes s ON m.SocketTypeId = s.Id;
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[RegisteredAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Configurations]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPUs]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPUs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Manufacturer] [nvarchar](100) NULL,
	[Cores] [int] NULL,
	[Threads] [int] NULL,
	[BaseClockGHz] [float] NULL,
	[BoostClockGHz] [float] NULL,
	[SocketTypeId] [int] NOT NULL,
	[TDP] [int] NULL,
	[Socket] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NULL,
	[PowerConsumption] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GPUs]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GPUs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Manufacturer] [nvarchar](100) NULL,
	[MemoryGB] [int] NULL,
	[MemoryType] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NULL,
	[PowerConsumption] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RAMs]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RAMs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CapacityGB] [int] NULL,
	[SpeedMHz] [int] NULL,
	[Type] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NULL,
	[PowerConsumption] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Storages]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Storages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[CapacityGB] [int] NULL,
	[Price] [decimal](10, 2) NULL,
	[PowerConsumption] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PSUs]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PSUs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Wattage] [int] NULL,
	[EfficiencyRating] [nvarchar](20) NULL,
	[Price] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cases]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[FormFactor] [nvarchar](50) NULL,
	[Color] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigurationComponents]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationComponents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConfigurationId] [int] NOT NULL,
	[CPUId] [int] NULL,
	[GPUId] [int] NULL,
	[RAMId] [int] NULL,
	[StorageId] [int] NULL,
	[MotherboardId] [int] NULL,
	[PSUId] [int] NULL,
	[CaseId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_FullConfigurations]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_FullConfigurations] AS
SELECT
    c.Id                AS ConfigurationId,
    c.Name              AS ConfigurationName,
    u.Email             AS OwnerEmail,
    cpu.Name            AS CPU,
    mb.Name             AS Motherboard,
    gpu.Name            AS GPU,
    ram.Name            AS RAM,
    st.Name             AS Storage,
    psu.Name            AS PSU,
    cs.Name             AS CaseName
FROM ConfigurationComponents cc
JOIN Configurations       c  ON cc.ConfigurationId = c.Id
JOIN Users                u  ON c.UserId             = u.Id
LEFT JOIN CPUs            cpu ON cc.CPUId             = cpu.Id
LEFT JOIN Motherboards    mb  ON cc.MotherboardId     = mb.Id
LEFT JOIN GPUs            gpu ON cc.GPUId             = gpu.Id
LEFT JOIN RAMs            ram ON cc.RAMId             = ram.Id
LEFT JOIN Storages        st  ON cc.StorageId         = st.Id
LEFT JOIN PSUs            psu ON cc.PSUId             = psu.Id
LEFT JOIN Cases           cs  ON cc.CaseId            = cs.Id;
GO
/****** Object:  Table [dbo].[ChipsetTypes]    Script Date: 2025. 07. 11. 0:43:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChipsetTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ChipsetName] [nvarchar](50) NOT NULL,
	[Manufacturer] [nvarchar](50) NOT NULL,
	[Features] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ChipsetName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cases] ADD  DEFAULT ('Black') FOR [Color]
GO
ALTER TABLE [dbo].[Cases] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[Configurations] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CPUs] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[CPUs] ADD  DEFAULT ((0)) FOR [PowerConsumption]
GO
ALTER TABLE [dbo].[GPUs] ADD  DEFAULT ('GDDR6') FOR [MemoryType]
GO
ALTER TABLE [dbo].[GPUs] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[GPUs] ADD  DEFAULT ((0)) FOR [PowerConsumption]
GO
ALTER TABLE [dbo].[PSUs] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[RAMs] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[RAMs] ADD  DEFAULT ((0)) FOR [PowerConsumption]
GO
ALTER TABLE [dbo].[Storages] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[Storages] ADD  DEFAULT ((0)) FOR [PowerConsumption]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [RegisteredAt]
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([ConfigurationId])
REFERENCES [dbo].[Configurations] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([CPUId])
REFERENCES [dbo].[CPUs] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([GPUId])
REFERENCES [dbo].[GPUs] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([MotherboardId])
REFERENCES [dbo].[Motherboards] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([PSUId])
REFERENCES [dbo].[PSUs] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([RAMId])
REFERENCES [dbo].[RAMs] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationComponents]  WITH CHECK ADD FOREIGN KEY([StorageId])
REFERENCES [dbo].[Storages] ([Id])
GO
ALTER TABLE [dbo].[Configurations]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[CPUs]  WITH CHECK ADD FOREIGN KEY([SocketTypeId])
REFERENCES [dbo].[SocketTypes] ([Id])
GO
ALTER TABLE [dbo].[Motherboards]  WITH CHECK ADD FOREIGN KEY([ChipsetTypeId])
REFERENCES [dbo].[ChipsetTypes] ([Id])
GO
ALTER TABLE [dbo].[Motherboards]  WITH CHECK ADD FOREIGN KEY([SocketTypeId])
REFERENCES [dbo].[SocketTypes] ([Id])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([Role]='admin' OR [Role]='user'))
GO
USE [master]
GO
ALTER DATABASE [PCConfiguratorDB] SET  READ_WRITE 
GO
