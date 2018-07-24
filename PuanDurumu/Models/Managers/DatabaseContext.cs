using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PuanDurumu.Models.Managers
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Maclar> Maclar { get; set; }

        public DbSet<Takimlar> Takimlar { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new DatabaseCreating());
        }

        public List<Takimlar> ExecuteGetDesplasmanTakimiSP(int evSahibi_Id)
        {

            return Database.SqlQuery<Takimlar>("EXEC GetDeplasmanTakimiSP @p0", evSahibi_Id).ToList();
        }

        public AtilanYenilenGol ExecuteGetAtilanYenilenGolSP(int takim_Id)
        {
            return Database.SqlQuery<AtilanYenilenGol>("EXEC GetAtilanYenilenGolSP @p0", takim_Id).ToList()[0];
        }
        
        public PuanGalibiyetMaglubiyetBeraberlik ExecuteGetPuanGalibiyetMaglubiyetBeraberlikSP(int takim_Id)
        {
            return Database.SqlQuery<PuanGalibiyetMaglubiyetBeraberlik>("EXEC GetPuanGalibiyetMaglubiyetBeraberlikSP @p0", takim_Id).ToList()[0];
        }

    }

    public  class AtilanYenilenGol
    {
        public int AtilanGol { get; set; }
        public int YenilenGol { get; set; }
    }

    public class PuanGalibiyetMaglubiyetBeraberlik
    {
        public int Puan { get; set; }
        public int Galibiyet { get; set; }
        public int Maglubiyet { get; set; }
        public int Beraberlik { get; set; }
    }

    public class DatabaseCreating : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            string[] sTakimlar = { "Galatasaray", "Fenerbahçe", "Beşiktaş", "Trabzonspor", "Real Madrid" };

            for (int i = 0; i < sTakimlar.Length; i++)
            {
                Takimlar takim = new Takimlar()
                {
                    Adi = sTakimlar[i]
                };

                context.Takimlar.Add(takim);
            }
            context.SaveChanges();

            //Maç Yapmamış Deplasman Takımlarını Getir
            context.Database.ExecuteSqlCommand(
                @"CREATE PROCEDURE GetDeplasmanTakimiSP
	                @p0 int
                AS
                BEGIN
	               select t.Id , t.Adi from Takimlar t 
	                where t.Id!=@p0 AND NOT EXISTS(select * from Maclar m WHERE m.DeplasmanTakimi_Id=T.Id AND m.EvSahibiTakimi_Id=@p0)
	                order by t.Adi
                END");

            //Toplam Atılan Golü Getirir
            context.Database.ExecuteSqlCommand(
                    @"CREATE PROCEDURE GetAtilanYenilenGolSP
	                @p0 int
                AS
                BEGIN
	              DECLARE @atilanGol int;
				DECLARE @yenilenGol int;
				set @atilanGol=0;
				set @yenilenGol=0;
				select @atilanGol=ISNULL(SUM(EvSkor),0) , @yenilenGol=ISNULL(SUM(DeplasmanSkor),0) from Maclar where EvSahibiTakimi_Id=@p0;

				select @atilanGol=@atilanGol+ISNULL(SUM(DeplasmanSkor),0) , @yenilenGol=@yenilenGol+ISNULL(SUM(EvSkor),0) from Maclar where DeplasmanTakimi_Id=@p0;

				Select @atilanGol  as 'AtilanGol' , @yenilenGol as 'YenilenGol'
                END"
                );

            //Puanı , Galibiyeti , Mağlubiyeti , Beraberliği getirir
            context.Database.ExecuteSqlCommand(
                @"CREATE PROCEDURE GetPuanGalibiyetMaglubiyetBeraberlikSP
			@p0 int
			AS
			BEGIN
			DECLARE @galibiyet int
			DECLARE @maglubiyet int
			DECLARE @beraberlik int
			set @galibiyet=0
			set @maglubiyet=0
			set @beraberlik=0
			DECLARE @EvSkor int
			DECLARE @DeplasmanSkor int
			DECLARE @DeplasmanTakimi_Id int
			DECLARE @EvSahibiTakimi_Id int
                 DECLARE CURSOR_MacSayilari CURSOR FOR Select EvSkor , DeplasmanSkor , DeplasmanTakimi_Id , EvSahibiTakimi_Id  from Maclar where DeplasmanTakimi_Id=@p0 OR EvSahibiTakimi_Id=@p0

        OPEN CURSOR_MacSayilari

        FETCH NEXT FROM CURSOR_MacSayilari INTO @EvSkor , @DeplasmanSkor , @DeplasmanTakimi_Id , @EvSahibiTakimi_Id

        WHILE @@FETCH_STATUS =0
	        BEGIN
		
		

		        IF @EvSahibiTakimi_Id = @p0
			        BEGIN
				        IF @EvSkor > @DeplasmanSkor
				        BEGIN
					        set @galibiyet = @galibiyet + 1
				        END
				        ELSE IF @EvSkor < @DeplasmanSkor
				        BEGIN
					        set @maglubiyet = @maglubiyet + 1
				        END
			        END
		        ELSE
			        BEGIN
				        IF @EvSkor > @DeplasmanSkor
				        BEGIN
					        set @maglubiyet = @maglubiyet + 1
				        END
				        ELSE IF @EvSkor < @DeplasmanSkor
				        BEGIN
					        set @galibiyet = @galibiyet + 1
				        END
			        END
		         IF @EvSkor = @DeplasmanSkor
			         BEGIN
				        set @beraberlik = @beraberlik + 1
			         END

		         FETCH NEXT FROM CURSOR_MacSayilari INTO @EvSkor , @DeplasmanSkor , @DeplasmanTakimi_Id , @EvSahibiTakimi_Id

	        END

	

	        CLOSE CURSOR_MacSayilari

	        DEALLOCATE CURSOR_MacSayilari

	        DECLARE @puan int
	
	        set @puan=0

	        set @puan= @puan + (@galibiyet*3)

	        set @puan= @puan + @beraberlik

	 
	        select @puan as 'Puan' , @galibiyet as 'Galibiyet' , @maglubiyet as 'Maglubiyet' , @beraberlik as 'Beraberlik'


            END"
                );
        }
    }
}