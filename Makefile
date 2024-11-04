# for development

build:
	dotnet build

run:
	dotnet run

daily:
	dotnet run daily

publish:
	dotnet publish -c Release -r linux-x64 

clean:
	dotnet clean
	@rm -rf Ressources/output/* LogFiles/* bin/ Ressources/first_scraping_done.txt