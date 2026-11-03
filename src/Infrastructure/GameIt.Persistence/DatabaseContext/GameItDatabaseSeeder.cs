using GameIt.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.DatabaseContext;

public static class GameItDatabaseSeeder
{
    public static async Task SeedAsync(GameItDbContext context)
    {
        var categoryMap = await EnsureCategoriesAsync(context);
        await EnsureGamesAndDiscountsAsync(context, categoryMap);
    }

    private static async Task<Dictionary<string, Guid>> EnsureCategoriesAsync(GameItDbContext context)
    {
        var existingCategories = await context.Categories.ToListAsync();
        var categoryMap = existingCategories.ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        var defaultCategories = new[]
        {
            "Action", "RPG", "Adventure", "Strategy", "Shooter",
            "Open World", "Indie", "Sports & Racing", "Horror", "Simulation"
        };

        var categoriesToAdd = new List<Category>();
        foreach (var categoryName in defaultCategories)
        {
            if (!categoryMap.ContainsKey(categoryName))
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName
                };
                categoriesToAdd.Add(category);
                categoryMap[categoryName] = category.Id;
            }
        }

        if (categoriesToAdd.Count > 0)
        {
            await context.Categories.AddRangeAsync(categoriesToAdd);
            await context.SaveChangesAsync();
        }

        return categoryMap;
    }

    private static async Task EnsureGamesAndDiscountsAsync(GameItDbContext context, Dictionary<string, Guid> categoryMap)
    {
        // Reconcile any existing rows seeded in bytes (> 1,000,000) to megabytes
        var legacyByteGames = await context.Games.Where(g => g.FileSizeInMB != null && g.FileSizeInMB > 1000000).ToListAsync();
        if (legacyByteGames.Count > 0)
        {
            foreach (var g in legacyByteGames)
            {
                g.FileSizeInMB = g.FileSizeInMB / (1024 * 1024);
            }
            await context.SaveChangesAsync();
        }

        var existingGameNames = await context.Games.Select(g => g.Name).ToListAsync();
        var existingSet = new HashSet<string>(existingGameNames, StringComparer.OrdinalIgnoreCase);

        var gamesList = GetPredefinedGames(categoryMap);
        var gamesToAdd = gamesList.Where(g => !existingSet.Contains(g.Name)).ToList();

        if (gamesToAdd.Count > 0)
        {
            await context.Games.AddRangeAsync(gamesToAdd);
            await context.SaveChangesAsync();

            // Seed active promotional discounts for select popular games
            var existingDiscounts = await context.Discounts.Select(d => d.GameId).ToListAsync();
            var discountSet = new HashSet<Guid>(existingDiscounts);

            var discountsToAdd = new List<Discount>();

            var discountedGames = new (string GameName, decimal Percentage, string Description)[]
            {
                ("Cyberpunk 2077", 50.00m, "Night City Half-Price Special"),
                ("The Witcher 3: Wild Hunt", 75.00m, "Franchise Celebration Sale"),
                ("Red Dead Redemption 2", 60.00m, "Outlaw Discount Week"),
                ("Grand Theft Auto V", 50.00m, "Los Santos Mid-Year Promotion"),
                ("God of War", 40.00m, "PlayStation PC Hit Discount"),
                ("Hogwarts Legacy", 30.00m, "Wizarding World Special Offer"),
                ("Resident Evil 4", 25.00m, "Capcom Survival Horror Deal"),
                ("Doom Eternal", 67.00m, "Hell on Earth Slayer Sale"),
                ("Horizon Zero Dawn Complete Edition", 75.00m, "Epic Post-Apocalyptic Bundle"),
                ("Slay the Spire", 66.00m, "Deckbuilder Indie Spotlight")
            };

            foreach (var deal in discountedGames)
            {
                var game = gamesToAdd.FirstOrDefault(g => g.Name.Equals(deal.GameName, StringComparison.OrdinalIgnoreCase))
                           ?? await context.Games.FirstOrDefaultAsync(g => g.Name == deal.GameName);

                if (game != null && !discountSet.Contains(game.Id))
                {
                    discountsToAdd.Add(new Discount
                    {
                        Id = Guid.NewGuid(),
                        GameId = game.Id,
                        Percentage = deal.Percentage,
                        Description = deal.Description,
                        StartDate = DateTime.UtcNow.AddDays(-7),
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        IsActive = true
                    });
                }
            }

            if (discountsToAdd.Count > 0)
            {
                await context.Discounts.AddRangeAsync(discountsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }

    private static List<Game> GetPredefinedGames(Dictionary<string, Guid> cat)
    {
        return new List<Game>
        {
            // --- RPG (7) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Cyberpunk 2077",
                Description = "An open-world, action-adventure RPG set in the megalopolis of Night City, where you play as a cyberpunk mercenary wrapped up in a do-or-die fight for survival.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1091500/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =70L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: 64-bit Windows 10 | Processor: Intel Core i7-6700 or AMD Ryzen 5 1600 | Memory: 12 GB RAM | Graphics: NVIDIA GeForce GTX 1060 6GB or AMD Radeon RX 580 8GB | Storage: 70 GB SSD",
                ReleaseDate = new DateTime(2020, 12, 10),
                CategoryId = cat["RPG"],
                PublisherId = "CD PROJEKT RED"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Elden Ring",
                Description = "Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between, crafted by Hidetaka Miyazaki and George R. R. Martin.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1245620/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =60L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10/11 | Processor: Intel Core i5-8400 or AMD Ryzen 3 3300X | Memory: 12 GB RAM | Graphics: NVIDIA GeForce GTX 1060 3GB or AMD Radeon RX 580 4GB | Storage: 60 GB",
                ReleaseDate = new DateTime(2022, 2, 25),
                CategoryId = cat["RPG"],
                PublisherId = "FromSoftware / Bandai Namco"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Baldur's Gate 3",
                Description = "Gather your party and return to the Forgotten Realms in a tale of fellowship, sacrifice, and survival, and the lure of absolute power. Unprecedented freedom to explore, experiment, and interact.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1086940/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =150L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel Core i7 4770K / AMD Ryzen 5 1500X | Memory: 16 GB RAM | Graphics: NVIDIA GTX 1060 6GB / AMD RX 580 | Storage: 150 GB SSD",
                ReleaseDate = new DateTime(2023, 8, 3),
                CategoryId = cat["RPG"],
                PublisherId = "Larian Studios"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "The Witcher 3: Wild Hunt",
                Description = "You are Geralt of Rivia, mercenary monster slayer. Before you stands a war-torn, monster-infested continent you can explore at will. Your current contract? Tracking down Ciri, the Child of Prophecy.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/292030/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =50L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: 64-bit Windows 7, 8, or 10 | Processor: Intel CPU Core i5-2500K / AMD A10-5800K | Memory: 6 GB RAM | Graphics: Nvidia GeForce GTX 660 / AMD Radeon HD 7870 | Storage: 50 GB",
                ReleaseDate = new DateTime(2015, 5, 18),
                CategoryId = cat["RPG"],
                PublisherId = "CD PROJEKT RED"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Starfield",
                Description = "In this next-generation role-playing game set amongst the stars, create any character you want and explore with unparalleled freedom as you embark on an epic journey to answer humanity's greatest mystery.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1716740/header.jpg",
                Price = 69.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =125L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10/11 | Processor: AMD Ryzen 5 2600X, Intel Core i7-6800K | Memory: 16 GB RAM | Graphics: AMD Radeon RX 5700, NVIDIA GeForce 1070 Ti | Storage: 125 GB SSD",
                ReleaseDate = new DateTime(2023, 9, 6),
                CategoryId = cat["RPG"],
                PublisherId = "Bethesda Softworks"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "The Elder Scrolls V: Skyrim Special Edition",
                Description = "Winner of more than 200 Game of the Year Awards, Skyrim Special Edition brings the epic fantasy to life with stunning detail. Play any type of character you can imagine and slay dragons in an open world.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/489830/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =12L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7/8.1/10 (64-bit) | Processor: Intel i5-750/AMD Phenom II X4-945 | Memory: 8 GB RAM | Graphics: NVIDIA GTX 470 1GB /AMD HD 7870 2GB | Storage: 12 GB",
                ReleaseDate = new DateTime(2016, 10, 27),
                CategoryId = cat["RPG"],
                PublisherId = "Bethesda Softworks"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Fallout 4",
                Description = "As the sole survivor of Vault 111, you enter a world destroyed by nuclear war. Every second is a fight for survival, and every choice is yours. Only you can rebuild and determine the fate of the Wasteland.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/377160/header.jpg",
                Price = 19.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =30L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7/8/10 (64-bit) | Processor: Intel Core i5-2300 2.8 GHz/AMD Phenom II X4 945 | Memory: 8 GB RAM | Graphics: NVIDIA GTX 550 Ti 2GB/AMD Radeon HD 7870 | Storage: 30 GB",
                ReleaseDate = new DateTime(2015, 11, 10),
                CategoryId = cat["RPG"],
                PublisherId = "Bethesda Softworks"
            },

            // --- Action (6) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Black Myth: Wukong",
                Description = "An action RPG rooted in Chinese mythology. You shall set out as the Destined One to venture into the challenges and marvels ahead, to uncover the obscured truth beneath the veil of a glorious legend.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2358720/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =130L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel Core i5-8400 / AMD Ryzen 5 1600 | Memory: 16 GB RAM | Graphics: NVIDIA GeForce GTX 1060 6GB / AMD Radeon RX 580 8GB | Storage: 130 GB",
                ReleaseDate = new DateTime(2024, 8, 20),
                CategoryId = cat["Action"],
                PublisherId = "Game Science"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "God of War",
                Description = "His vengeance against the Gods of Olympus years behind him, Kratos now lives as a man in the realm of Norse Gods and monsters. It is in this harsh, unforgiving world that he must fight to survive… and teach his son to do the same.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1593500/header.jpg",
                Price = 49.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =70L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel i5-6600k / AMD Ryzen 5 2400 G | Memory: 8 GB RAM | Graphics: NVIDIA GTX 1060 (6 GB) / AMD RX 570 (4 GB) | Storage: 70 GB SSD",
                ReleaseDate = new DateTime(2022, 1, 14),
                CategoryId = cat["Action"],
                PublisherId = "PlayStation PC LLC"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Sekiro: Shadows Die Twice",
                Description = "Carve your own clever path to vengeance in this critically acclaimed adventure from developer FromSoftware. Take revenge. Restore your honor. Kill ingeniously.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/814380/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =25L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 64-bit | Processor: Intel Core i3-2100 | AMD FX-6300 | Memory: 4 GB RAM | Graphics: NVIDIA GeForce GTX 760 | AMD Radeon HD 7950 | Storage: 25 GB",
                ReleaseDate = new DateTime(2019, 3, 21),
                CategoryId = cat["Action"],
                PublisherId = "Activision"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Dark Souls III",
                Description = "As fires fade and the world falls into ruin, journey into a universe filled with more colossal enemies and environments. Fans and newcomers alike will get lost in the hallmark gameplay and immersive graphics.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/374320/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =25L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 SP1 64bit | Processor: Intel Core i3-2100 / AMD FX-6300 | Memory: 4 GB RAM | Graphics: NVIDIA GeForce GTX 750 Ti / ATI Radeon HD 7950 | Storage: 25 GB",
                ReleaseDate = new DateTime(2016, 4, 11),
                CategoryId = cat["Action"],
                PublisherId = "Bandai Namco"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Monster Hunter: World",
                Description = "Welcome to a new world! Take on the role of a hunter and slay ferocious monsters in a living, breathing ecosystem where you can use the landscape and its diverse inhabitants to get the upper hand.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/582010/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =48L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: WINDOWS 7, 8, 8.1, 10 (64-bit) | Processor: Intel Core i5-4460 / AMD FX-6300 | Memory: 8 GB RAM | Graphics: NVIDIA GeForce GTX 760 or AMD Radeon R7 260x | Storage: 48 GB",
                ReleaseDate = new DateTime(2018, 8, 9),
                CategoryId = cat["Action"],
                PublisherId = "Capcom"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Armored Core VI Fires of Rubicon",
                Description = "Combining FromSoftware's longstanding expertise in mech games with their signature action gameplay, ARMORED CORE VI FIRES OF RUBICON brings a brand-new high-octane action experience to the franchise.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1888160/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =60L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 | Processor: Intel Core i7-4790K / AMD Ryzen 7 1800X | Memory: 12 GB RAM | Graphics: NVIDIA GeForce GTX 1650 4GB / AMD Radeon RX 480 4GB | Storage: 60 GB",
                ReleaseDate = new DateTime(2023, 8, 24),
                CategoryId = cat["Action"],
                PublisherId = "FromSoftware / Bandai Namco"
            },

            // --- Open World (5) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Red Dead Redemption 2",
                Description = "Winner of over 175 Game of the Year Awards and recipient of over 250 perfect scores, RDR2 is the epic tale of outlaw Arthur Morgan and the infamous Van der Linde gang on the run across the rugged American heartland.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1174180/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =150L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 | Processor: Intel Core i7-4770K / AMD Ryzen 5 1500X | Memory: 12 GB RAM | Graphics: Nvidia GeForce GTX 1060 6GB / AMD Radeon RX 480 4GB | Storage: 150 GB",
                ReleaseDate = new DateTime(2019, 12, 5),
                CategoryId = cat["Open World"],
                PublisherId = "Rockstar Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Grand Theft Auto V",
                Description = "When a young street hustler, a retired bank robber and a terrifying psychopath find themselves entangled with some of the most frightening and deranged elements of the criminal underworld, they must pull off dangerous heists.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/271590/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =110L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64 Bit | Processor: Intel Core i5 3470 @ 3.2GHz / AMD X8 FX-8350 @ 4GHz | Memory: 8 GB RAM | Graphics: NVIDIA GTX 660 2GB / AMD HD 7870 2GB | Storage: 110 GB",
                ReleaseDate = new DateTime(2015, 4, 14),
                CategoryId = cat["Open World"],
                PublisherId = "Rockstar Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Ghost of Tsushima DIRECTOR'S CUT",
                Description = "In the late 13th century, the Mongol empire has laid waste to entire nations. Tsushima Island is all that stands between mainland Japan and a massive Mongol invasion fleet. Jin Sakai must forge a new path: the Ghost.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2215430/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =75L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel Core i5-8600 / AMD Ryzen 5 3600 | Memory: 16 GB RAM | Graphics: NVIDIA GeForce RTX 2060 / AMD Radeon RX 5600 XT | Storage: 75 GB SSD",
                ReleaseDate = new DateTime(2024, 5, 16),
                CategoryId = cat["Open World"],
                PublisherId = "PlayStation PC LLC"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Hogwarts Legacy",
                Description = "An immersive, open-world action RPG set in the world first introduced in the Harry Potter books. Experience Hogwarts in the 1800s. Your character is a student who holds the key to an ancient secret.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/990080/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =85L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: 64-bit Windows 10 | Processor: Intel Core i5-6600 or AMD Ryzen 5 1400 | Memory: 16 GB RAM | Graphics: NVIDIA GeForce GTX 960 4GB or AMD Radeon RX 470 4GB | Storage: 85 GB",
                ReleaseDate = new DateTime(2023, 2, 10),
                CategoryId = cat["Open World"],
                PublisherId = "Warner Bros. Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Marvel's Spider-Man Remastered",
                Description = "In Marvel's Spider-Man Remastered, the worlds of Peter Parker and Spider-Man collide in an original action-packed story. Play as an experienced Peter Parker, fighting big crime and iconic villains in New York.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1817070/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =75L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel Core i5-4670, 3.4 Ghz or AMD FX-8350, 4.2 Ghz | Memory: 16 GB RAM | Graphics: NVIDIA GeForce GTX 1060 6GB or AMD Radeon RX 580 8GB | Storage: 75 GB SSD",
                ReleaseDate = new DateTime(2022, 8, 12),
                CategoryId = cat["Open World"],
                PublisherId = "PlayStation PC LLC"
            },

            // --- Shooter (6) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Helldivers 2",
                Description = "The Galaxy's Last Line of Offence. Enlist in the Helldivers and join the fight for freedom across a hostile galaxy in a fast, frantic, and ferocious third-person co-op shooter.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/553850/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =100L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 | Processor: Intel Core i7-9700K or AMD Ryzen 7 3700X | Memory: 16 GB RAM | Graphics: NVIDIA GeForce RTX 2060 or AMD Radeon RX 6600XT | Storage: 100 GB",
                ReleaseDate = new DateTime(2024, 2, 8),
                CategoryId = cat["Shooter"],
                PublisherId = "PlayStation PC LLC"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Doom Eternal",
                Description = "Hell's armies have invaded Earth. Become the Slayer in an epic single-player campaign to conquer demons across dimensions and stop the final destruction of humanity. The only thing they fear... is you.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/782330/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =50L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: 64-bit Windows 10 | Processor: Intel Core i7-6700K or AMD Ryzen 7 1800X | Memory: 8 GB RAM | Graphics: NVIDIA GeForce GTX 1060 (6GB), AMD Radeon RX 480 (8GB) | Storage: 50 GB",
                ReleaseDate = new DateTime(2020, 3, 19),
                CategoryId = cat["Shooter"],
                PublisherId = "Bethesda Softworks"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Counter-Strike 2",
                Description = "For over two decades, Counter-Strike has offered an elite competitive experience, one shaped by millions of players from across the globe. Now the next chapter in the CS story begins: Counter-Strike 2.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/730/header.jpg",
                Price = 0.00m,
                IsFree = true,
                IsFeatured = false,
                FileSizeInMB =35L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 (64 bits) | Processor: 4 hardware CPU threads - Intel Core i5 750 or higher | Memory: 8 GB RAM | Graphics: Video card must be 1 GB or more with Shader Model 5.0 | Storage: 85 GB",
                ReleaseDate = new DateTime(2023, 9, 27),
                CategoryId = cat["Shooter"],
                PublisherId = "Valve"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Apex Legends",
                Description = "Conquer with character in Apex Legends, a free-to-play Hero shooter where legendary characters with powerful abilities team up to battle for fame & fortune on the fringes of the Frontier.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1172470/header.jpg",
                Price = 0.00m,
                IsFree = true,
                IsFeatured = false,
                FileSizeInMB =75L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: 64-bit Windows 10 | Processor: Intel i5 3570K or equivalent | Memory: 8 GB RAM | Graphics: Nvidia GeForce GTX 970 / AMD Radeon R9 290 | Storage: 75 GB",
                ReleaseDate = new DateTime(2020, 11, 4),
                CategoryId = cat["Shooter"],
                PublisherId = "Electronic Arts"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Destiny 2",
                Description = "Dive into the world of Destiny 2 to explore the mysteries of the solar system and experience responsive first-person shooter combat. Unlock powerful elemental abilities and collect unique gear.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1085660/header.jpg",
                Price = 0.00m,
                IsFree = true,
                IsFeatured = false,
                FileSizeInMB =105L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel Core i5 2400 3.4 GHz or AMD FX 4350 4.2 GHz | Memory: 8 GB RAM | Graphics: NVIDIA GeForce GTX 660 2GB or AMD Radeon HD 7850 2GB | Storage: 105 GB",
                ReleaseDate = new DateTime(2019, 10, 1),
                CategoryId = cat["Shooter"],
                PublisherId = "Bungie"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Deep Rock Galactic",
                Description = "A 1-4 player co-op FPS featuring badass space Dwarves, 100% destructible environments, procedurally-generated caves, and endless hordes of alien monsters. Danger, Darkness, Dwarves!",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/548430/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =3L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 64 Bit | Processor: 2.4 GHz Dual Core | Memory: 6 GB RAM | Graphics: NVIDIA GeForce GTX 460 or AMD Radeon HD 5770 | Storage: 3 GB",
                ReleaseDate = new DateTime(2020, 5, 13),
                CategoryId = cat["Shooter"],
                PublisherId = "Coffee Stain Publishing"
            },

            // --- Adventure (5) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Horizon Zero Dawn Complete Edition",
                Description = "Experience Aloy's entire legendary quest to unravel the mysteries of a world ruled by deadly Machines. An outcast from her tribe, the young hunter fights to uncover her past and stop a catastrophic threat.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1151640/header.jpg",
                Price = 49.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =70L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bits | Processor: Intel Core i7-4770K@3.5GHz or Ryzen 5 1500X@3.5GHz | Memory: 16 GB RAM | Graphics: Nvidia GeForce GTX 1060 (6 GB) or AMD Radeon RX 580 (8 GB) | Storage: 70 GB",
                ReleaseDate = new DateTime(2020, 8, 7),
                CategoryId = cat["Adventure"],
                PublisherId = "PlayStation PC LLC"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Death Stranding Director's Cut",
                Description = "From legendary game creator Hideo Kojima comes a genre-defying experience. In the future, a mysterious event known as the Death Stranding has opened a doorway between the living and the dead.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1850570/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =80L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 | Processor: Intel Core i7-3770 or AMD Ryzen 5 1600 | Memory: 8 GB RAM | Graphics: GeForce GTX 1060 6 GB or AMD Radeon RX 590 | Storage: 80 GB",
                ReleaseDate = new DateTime(2022, 3, 30),
                CategoryId = cat["Adventure"],
                PublisherId = "505 Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "It Takes Two",
                Description = "Embark on the craziest journey of your life in It Takes Two, a genre-bending platform adventure created purely for co-op. Invite a friend to join for free with Friend's Pass across joyfully disruptive challenges.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1426210/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =50L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 8.1 64-bit or Windows 10 64-bit | Processor: Intel Core i5 3570K or AMD FX-8350 | Memory: 16 GB RAM | Graphics: Nvidia GeForce GTX 980 or AMD Radeon R9 290X | Storage: 50 GB",
                ReleaseDate = new DateTime(2021, 3, 26),
                CategoryId = cat["Adventure"],
                PublisherId = "Electronic Arts"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Subnautica",
                Description = "Descend into the depths of an alien underwater world filled with wonder and peril. Craft equipment, pilot submarines and out-smart wildlife to explore lush coral reefs, volcanoes, cave systems, and more.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/264710/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =20L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows Vista SP2 or newer, 64-bit | Processor: Intel Haswell 4 cores / 4 threads @ 3.2Ghz | Memory: 8 GB RAM | Graphics: Nvidia GTX 550 Ti or equivalent | Storage: 20 GB",
                ReleaseDate = new DateTime(2018, 1, 23),
                CategoryId = cat["Adventure"],
                PublisherId = "Unknown Worlds Entertainment"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Palworld",
                Description = "Fight, farm, build and work alongside mysterious creatures called 'Pals' in this completely new multiplayer, open-world survival and crafting game.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1623730/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =40L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 or later (64-Bit) | Processor: i9-9900K 3.6GHz 8 Core | Memory: 32 GB RAM | Graphics: GeForce RTX 2070 | Storage: 40 GB SSD",
                ReleaseDate = new DateTime(2024, 1, 19),
                CategoryId = cat["Adventure"],
                PublisherId = "Pocketpair"
            },

            // --- Horror (4) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Resident Evil 4",
                Description = "Survival is only the beginning. Six years have passed since the biological disaster in Raccoon City. Leon S. Kennedy, one of the survivors, tracks the president's kidnapped daughter to a secluded European village.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2050650/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =68L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 (64 bit) | Processor: AMD Ryzen 5 3600 / Intel Core i7 8700 | Memory: 16 GB RAM | Graphics: AMD Radeon RX 5700 / NVIDIA GeForce GTX 1070 | Storage: 68 GB",
                ReleaseDate = new DateTime(2023, 3, 24),
                CategoryId = cat["Horror"],
                PublisherId = "Capcom"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Resident Evil Village",
                Description = "Experience survival horror like never before in the eighth major installment in the storied Resident Evil franchise - Resident Evil Village. Set a few years after the horrifying events in Resident Evil 7 biohazard.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1196590/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =35L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 (64 bit) | Processor: AMD Ryzen 5 3600 or Intel Core i7 8700 | Memory: 16 GB RAM | Graphics: AMD Radeon RX 5700 or NVIDIA GeForce GTX 1070 | Storage: 35 GB",
                ReleaseDate = new DateTime(2021, 5, 7),
                CategoryId = cat["Horror"],
                PublisherId = "Capcom"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Resident Evil 2",
                Description = "A deadly virus engulfs the residents of Raccoon City in September of 1998, plunging the town into chaos as flesh eating zombies roam the streets for survivors. An unparalleled adrenaline rush.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/883710/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =26L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: WINDOWS 7, 8.1, 10 (64-BIT) | Processor: Intel Core i7-3770 or AMD FX-9590 | Memory: 8 GB RAM | Graphics: NVIDIA GeForce GTX 1060 or AMD Radeon RX 480 | Storage: 26 GB",
                ReleaseDate = new DateTime(2019, 1, 25),
                CategoryId = cat["Horror"],
                PublisherId = "Capcom"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Dead Space",
                Description = "The sci-fi survival horror classic returns, completely rebuilt from the ground up to offer a deeper and more immersive experience. Isaac Clarke is an engineer on a mission to repair a vast Mining Ship, the USG Ishimura.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1693980/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =50L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Window 10 64-bit + | Processor: Ryzen 5 5600X,Core i5 11600K | Memory: 16 GB RAM | Graphics: Radeon RX 6700 XT, Geforce RTX 2070 | Storage: 50 GB SSD",
                ReleaseDate = new DateTime(2023, 1, 27),
                CategoryId = cat["Horror"],
                PublisherId = "Electronic Arts"
            },

            // --- Indie (8) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Hades",
                Description = "Defy the god of the dead as you hack and slash out of the Underworld in this rogue-like dungeon crawler from the creators of Bastion, Transistor, and Pyre.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1145360/header.jpg",
                Price = 24.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =15L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 SP1 | Processor: Dual Core 3.0 Ghz | Memory: 4 GB RAM | Graphics: 1GB VRAM / DirectX 10+ support | Storage: 15 GB",
                ReleaseDate = new DateTime(2020, 9, 17),
                CategoryId = cat["Indie"],
                PublisherId = "Supergiant Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Hades II",
                Description = "Battle beyond the Underworld using dark sorcery to take on the Titan of Time in this bewitching sequel to the award-winning rogue-like dungeon crawler.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1145350/header.jpg",
                Price = 29.99m,
                IsFree = false,
                IsFeatured = true,
                FileSizeInMB =10L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Dual Core 2.4 GHz | Memory: 8 GB RAM | Graphics: GeForce GTX 950, Radeon R7 360, or Intel HD Graphics 630 | Storage: 10 GB",
                ReleaseDate = new DateTime(2024, 5, 6),
                CategoryId = cat["Indie"],
                PublisherId = "Supergiant Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Hollow Knight",
                Description = "Forge your own path in Hollow Knight! An epic action adventure through a vast ruined kingdom of insects and heroes. Explore twisting caverns, battle tainted creatures and befriend bizarre bugs.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/367520/header.jpg",
                Price = 14.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =9L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 (64bit) | Processor: Intel Core 2 Duo E5200 | Memory: 4 GB RAM | Graphics: GeForce 9800GTX+ (1GB) | Storage: 9 GB",
                ReleaseDate = new DateTime(2017, 2, 24),
                CategoryId = cat["Indie"],
                PublisherId = "Team Cherry"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Dead Cells",
                Description = "Dead Cells is a rogue-lite, metroidvania inspired, action-platformer. You'll explore a sprawling, ever-changing castle... assuming you’re able to fight your way past its keepers in 2D souls-lite combat.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/588650/header.jpg",
                Price = 24.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =2L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7+ | Processor: Intel i5+ | Memory: 4 GB RAM | Graphics: Nvidia GTX 460 / Radeon HD 7800 | Storage: 2 GB",
                ReleaseDate = new DateTime(2018, 8, 7),
                CategoryId = cat["Indie"],
                PublisherId = "Motion Twin"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Celeste",
                Description = "Help Madeline survive her inner demons on her journey to the top of Celeste Mountain, in this super-tight, hand-crafted platformer from the creators of multiplayer classic TowerFall.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/504230/header.jpg",
                Price = 19.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB = 1200L,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 or newer | Processor: Intel Core i3 M380 | Memory: 2 GB RAM | Graphics: Intel HD 4000 | Storage: 1200 MB",
                ReleaseDate = new DateTime(2018, 1, 25),
                CategoryId = cat["Indie"],
                PublisherId = "Maddy Makes Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Slay the Spire",
                Description = "We fused card games and roguelikes together to make the best single player deckbuilder we could. Craft a unique deck, encounter bizarre creatures, discover relics of immense power, and Slay the Spire!",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/646570/header.jpg",
                Price = 24.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =1L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7, 8/8.1, 10 | Processor: 2.0 Ghz | Memory: 2 GB RAM | Graphics: 128mb Video Memory, capable of Shader Model 2.0+ | Storage: 1 GB",
                ReleaseDate = new DateTime(2019, 1, 23),
                CategoryId = cat["Indie"],
                PublisherId = "Mega Crit Games"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Stardew Valley",
                Description = "You've inherited your grandfather's old farm plot in Stardew Valley. Armed with hand-me-down tools and a few coins, you set out to begin your new life. Can you learn to live off the land?",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/413150/header.jpg",
                Price = 14.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB = 500L,
                DownloadLink = null,
                SystemRequirements = "OS: Windows Vista or greater | Processor: 2 Ghz | Memory: 2 GB RAM | Graphics: 256 mb video memory, shader model 3.0+ | Storage: 500 MB",
                ReleaseDate = new DateTime(2016, 2, 26),
                CategoryId = cat["Indie"],
                PublisherId = "ConcernedApe"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Terraria",
                Description = "Dig, fight, explore, build! Nothing is impossible in this action-packed adventure game. The world is your canvas and the ground itself is your paint. Delve deep into cavernous expanses in search of treasure and raw materials.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/105600/header.jpg",
                Price = 9.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB = 200L,
                DownloadLink = null,
                SystemRequirements = "OS: Windows Xp, Vista, 7, 8/8.1, 10 | Processor: 2.0 Ghz | Memory: 2.5 GB RAM | Graphics: 128mb Video Memory | Storage: 200 MB",
                ReleaseDate = new DateTime(2011, 5, 16),
                CategoryId = cat["Indie"],
                PublisherId = "Re-Logic"
            },

            // --- Strategy (5) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Civilization VI",
                Description = "Civilization VI offers new ways to interact with your world: cities now physically expand across the map, active research in technology and culture unlocks new potential, and competing leaders pursue their own agendas.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/289070/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =15L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7x64 / Windows 8.1x64 / Windows 10x64 | Processor: Fourth Generation Intel Core i5 2.5 Ghz or AMD FX8350 4.0 Ghz | Memory: 8 GB RAM | Graphics: 2GB DirectX 11 Video Card | Storage: 15 GB",
                ReleaseDate = new DateTime(2016, 10, 21),
                CategoryId = cat["Strategy"],
                PublisherId = "2K"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Age of Empires IV: Anniversary Edition",
                Description = "Celebrating its first year of delighting millions of global players, the award-winning and best-selling strategy franchise continues with Age of Empires IV: Anniversary Edition, putting you at the center of epic historical battles.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1466860/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =50L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64bit | Windows 11 64bit | Processor: Intel i5-6300HQ or AMD Ryzen 3 1300X | Memory: 16 GB RAM | Graphics: GeForce GTX 970 or Radeon RX 570 | Storage: 50 GB",
                ReleaseDate = new DateTime(2021, 10, 28),
                CategoryId = cat["Strategy"],
                PublisherId = "Xbox Game Studios"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Total War: WARHAMMER III",
                Description = "The cataclysmic conclusion to the Total War: WARHAMMER trilogy is here. Rally your forces and step into the Realm of Chaos, a dimension of mind-bending horror where the very fate of the world will be decided.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1142710/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =120L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit | Processor: Intel i5/Ryzen 5 series | Memory: 8 GB RAM | Graphics: Nvidia GeForce GTX 1660 Ti/AMD Radeon RX 5600-XT | Storage: 120 GB",
                ReleaseDate = new DateTime(2022, 2, 17),
                CategoryId = cat["Strategy"],
                PublisherId = "SEGA"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Stellaris",
                Description = "Explore a galaxy full of wonders in this sci-fi grand strategy game from Paradox Development Studio. Interact with diverse alien races, discover strange new worlds with unexpected events and expand the reach of your empire.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/281990/header.jpg",
                Price = 39.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =10L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 SP1 64 Bit | Processor: Intel Core i5-3570K or AMD FX-6350 | Memory: 4 GB RAM | Graphics: Nvidia GeForce GTX 560 Ti or AMD Radeon HD 7850 | Storage: 10 GB",
                ReleaseDate = new DateTime(2016, 5, 9),
                CategoryId = cat["Strategy"],
                PublisherId = "Paradox Interactive"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Crusader Kings III",
                Description = "Love, fight, scheme, and claim greatness. Determine your noble house's legacy in the sprawling grand strategy of Crusader Kings III. Death is only the beginning as you guide your dynasty's bloodline through the Middle Ages.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1158310/header.jpg",
                Price = 49.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =12L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 Home 64 bit | Processor: Intel Core i5-4670K / AMD Ryzen 5 2400G | Memory: 8 GB RAM | Graphics: Nvidia GeForce GTX 1650 (4 GB) / AMD Radeon R9 390X (8 GB) | Storage: 12 GB",
                ReleaseDate = new DateTime(2020, 9, 1),
                CategoryId = cat["Strategy"],
                PublisherId = "Paradox Interactive"
            },

            // --- Sports & Racing (4) ---
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Forza Horizon 5",
                Description = "Your Ultimate Horizon Adventure awaits! Explore the vibrant and ever-evolving open world landscapes of Mexico with limitless, fun driving action in hundreds of the world's greatest cars.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1551360/header.jpg",
                Price = 59.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =110L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 version 15063.0 or higher | Processor: Intel i5-8400 or AMD Ryzen 5 1500X | Memory: 16 GB RAM | Graphics: NVidia GTX 1070 OR AMD RX 590 | Storage: 110 GB",
                ReleaseDate = new DateTime(2021, 11, 9),
                CategoryId = cat["Sports & Racing"],
                PublisherId = "Xbox Game Studios"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "EA SPORTS FC 24",
                Description = "EA SPORTS FC 24 is a new era for The World's Game: 19,000+ fully licensed players, 700+ teams, and 30+ leagues playing together in the most authentic football experience ever created on PC.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2195250/header.jpg",
                Price = 69.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =100L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 - 64-Bit | Processor: Intel Core i7-6700 @ 3.40GHz or AMD Ryzen 7 2700X @ 3.7GHz | Memory: 12 GB RAM | Graphics: NVIDIA GeForce GTX 1660 or AMD RX 5600 XT | Storage: 100 GB",
                ReleaseDate = new DateTime(2023, 9, 29),
                CategoryId = cat["Sports & Racing"],
                PublisherId = "Electronic Arts"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "F1 23",
                Description = "Be the last to brake in EA SPORTS F1 23, the official video game of the 2023 FIA Formula One World Championship. A new chapter in the thrilling 'Braking Point' story mode delivers high-speed drama.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2108330/header.jpg",
                Price = 69.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =80L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 10 64-bit (Version 21H1 or higher) | Processor: Intel Core i5 9600K or AMD Ryzen 5 2600X | Memory: 16 GB RAM | Graphics: NVIDIA GTX 1660 Ti or AMD RX 590 | Storage: 80 GB",
                ReleaseDate = new DateTime(2023, 6, 16),
                CategoryId = cat["Sports & Racing"],
                PublisherId = "Electronic Arts"
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Name = "Assetto Corsa",
                Description = "Assetto Corsa features an advanced DirectX 11 graphics engine that recreates an immersive environment, dynamic lighting, realistic materials, and laser-scanned circuits for authentic racing simulation.",
                ImageUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/244210/header.jpg",
                Price = 19.99m,
                IsFree = false,
                IsFeatured = false,
                FileSizeInMB =30L * 1024,
                DownloadLink = null,
                SystemRequirements = "OS: Windows 7 Sp1 - 8 - 8.1 - 10 | Processor: AMD Six-Core CPU, Intel Quad-Core CPU | Memory: 6 GB RAM | Graphics: DirectX 11 (e.g. AMD Radeon 290x, Nvidia GeForce GTX 970) | Storage: 30 GB",
                ReleaseDate = new DateTime(2014, 12, 19),
                CategoryId = cat["Sports & Racing"],
                PublisherId = "505 Games"
            }
        };
    }
}
