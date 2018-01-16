# Dss make file
RM		:= rm -rf
CP		:= cp
MKDIR		:= mkdir -p
NUNIT		:= nunit-console-x86
CHMOD		:= chmod a+x

SRCDIR		:= ../src
LIBDIR		:= ../lib
BINDIR		:= ../bin
WEBDIR		:= ../tmp/Web
RELEASEDIR	:= ../release
BUILDDIR	:= ../build
TESTDIR		:= ../build/Test
DTGDIR		:= ../bin/DTG
NANTDIR		:= ../bin/NAnt

RELEASE_REVISION:= 14309
BRANCH_TAG 	:= HEAD
SVN_BRANCH_TAG 	:= trunk
TARGET_PROFILE := Stage

GETVERSION	:= $(BINDIR)/getversion.sh

# Third party assemblies.
LOG4NET		:= $(BUILDDIR)/log4net.dll
NUNIT_FRAMEWORK := $(BUILDDIR)/nunit.framework.dll


DOTNETFRAMEWORK	:= c:/WINDOWS/Microsoft.NET/Framework/v4.0.30319
#MSBUILD		:= $(DOTNETFRAMEWORK)/MSBuild.exe
MSBUILD		:= "C:/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe"
REGSVCS		:= $(DOTNETFRAMEWORK)/Regsvcs.exe
7ZIP		:= $(BINDIR)/7za.exe

all:

include ./*.mk

.DELETE_ON_ERROR:

all: build_db build

.PHONY: build 
build: msbuild
	@#aspnetc

msbuild:
	$(MSBUILD) Cortside.Common.sln /target:Build 

build_tests:
	#$(MSBUILD) $(CLIENT_NAME).Dss.Test.csproj $(CLIENT_NAME).Dss.Web.Portal.Controller.UnitTest.csproj $(CLIENT_NAME).Dss.Web.Personal.Controller.UnitTest.csproj $(CLIENT_NAME).Dss.Web.Common.AjaxCommand.UnitTest.csproj $(CLIENT_NAME).Dss.Web.Common.Controller.UnitTest.csproj $(CLIENT_NAME).Dss.UnitTest.csproj $(CLIENT_NAME)/.Dss.BusinessLogic.UnitTest.csproj /verbosity:minimal /target:Build

.PHONY: clean
clean:
	rm -rf Cortside.Common.sln.cache
	$(MSBUILD) Cortside.Common.sln /verbosity:minimal /target:Clean
	$(RM) $(BUILDDIR)/*
	$(RM) Web/build
	$(RM) obj
	$(RM) bin
	$(RM) ccnet-nunit-results.xml
	$(RM) TestResult.xml


.PHONY: precommit
precommit: build build_db regress_UnitTests

.PHONY: gen
gen: generate updateProjectFiles

generate:
	@$(DTGDIR)/DataTierGenerator.exe -f DataTierGenerator.config.xml

updateProjectFiles:	
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Data.csproj -d Data/Model
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Data.csproj -d Data/Enum
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Domain.csproj -d Domain/Entity
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.DomainService.csproj -d DomainService
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.DomainService.Implementor.csproj -d DomainService/Implementor
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Repository.csproj -d Repository
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Repository.NHRepository.csproj -d NHRepository
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Test.Domain.csproj -d Test/Domain
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Test.DomainService.csproj -d Test/DomainService
	@$(DTGDIR)/UpdateProjectFiles.exe -p Cortside.Common.Test.NHRepository.csproj -d Test/NHRepository

.PHONY: ncover
ncover: 
	regsvr32 ../bin/ncover/CoverLib.dll /s
	rm -rf ../docs/NCover
	mkdir -p ../docs/NCover
	../bin/NCover/NCover.Console.exe //reg //l ../docs/NCover/Coverage.log //x ../docs/NCover/Coverage.xml ../bin/nunit/nunit-console-x86.exe /xml=ccnet-nunit-results.xml ../build/Test/$(CLIENT_NAME).Dss.Test.dll 
	
ncoverreports:
	#../bin/nxslt ../docs/NCover/Coverage.xml ../docs/NCover/Coverage.xsl  -o ../docs/NCover/Coverage.html
	../bin/NCover/NCoverReport.exe ../docs/NCover/Coverage.xml
	cp ../bin/NCover/NCoverReport.css ../docs/NCover/
	
.PHONY: regress
regress: build nunit

nunit:
	#$(BINDIR)/NUnit/nunit-console-x86 /xml=ccnet-nunit-results-db.xml ../build/Test/Cortside.Common.Test.NHRepository.dll ../build/Test/Cortside.Common.Test.Integration.dll

.PHONY: regress_UnitTests
regress_UnitTests :
	echo '<appSettings><add key="ConnectionString" value="" /></appSettings>' > user.config.foo
ifeq ($(shell cat user.config | wc -c), 67)
	rm user.config
else
	mv user.config user.config.hold
endif
	mv user.config.foo user.config
	#$(BINDIR)/NUnit/nunit-console-x86 /xml=ccnet-nunit-results.xml ../build/Test/Cortside.Common.Test.ApplicationService.dll ../build/Test/Cortside.Common.Test.Domain.dll ../build/Test/Cortside.Common.Test.DomainService.dll ../build/Test/Cortside.Common.Test.Web.CustomerSite.dll ../build/Test/Cortside.Common.Test.DTOAssembler.dll ../build/Test/Cortside.Common.Test.Web.MerchantSite.dll ../build/Test/Cortside.Common.Test.Web.Common.dll
	rm user.config
	-mv user.config.hold user.config
	
changelog:
	../bin/svn2cl/svn2cl.sh -o=../build/ChangeLog.txt

.PHONY: release
release: package tagversion

.PHONY: package
xpackage: clean build packageonly 
	#cp ../ChangeLog.txt ${RELEASEDIR}/ChangeLog-$(shell $(GETVERSION)).txt

package: build precommit
	./package.sh Cortside.Common.BootStrap $(VERSION)
	./package.sh Cortside.Common.Command $(VERSION)
	./package.sh Cortside.Common.IoC $(VERSION)
	./package.sh Cortside.Common.Message $(VERSION)
	./package.sh Cortside.Common.Query $(VERSION)
	./package.sh Cortside.Common.RabbitMQ $(VERSION)
	./package.sh Cortside.Common.Web.Mvc $(VERSION)
	./package.sh Cortside.Common.Web.Security $(VERSION)
	./package.sh Cortside.Common.Data.Exceptions $(VERSION)	
	
packageonly: package_prep package_all package_postop 

package_prep:
	$(MKDIR) ${RELEASEDIR}
	rm -rf ../tmp
	$(MKDIR) ../tmp

package_all:
	xcopy ..\\build\\*.dll ..\\tmp /S /C /I /Q /Y 
	xcopy ..\\build\\*.pdb ..\\tmp /S /C /I /Q /Y 
	cd ../tmp; ${7ZIP} a ${RELEASEDIR}/Cortside.Common-$(shell $(GETVERSION)).7z * -r
	rm -rf ../tmp

package_postop:
	rm -rf ../tmp


VERSION := $(subst .,_,$(shell $(GETVERSION)))

	
.PHONY: tagbuild
tagbuild:
	#svn copy -m "BUILD_$(VERSION)" $(SVN_URL)$(CLIENT_NAME)/trunk $(SVN_URL)$(CLIENT_NAME)/tags/BUILD_$(VERSION)

.PHONY: tagversion
tagversion:
	#svn copy -m "RELEASE_$(VERSION)" $(SVN_URL)$(CLIENT_NAME)/trunk $(SVN_URL)$(CLIENT_NAME)/tags/RELEASE_$(VERSION)

.PHONY: updateversion
updateversion: 
	${BINDIR}/UpdateVersion -b Increment -i AssemblyVersionInfo.cs -o AssemblyVersionInfo.cs

	
%version.aspx: AssemblyVersionInfo.cs
	#sed -e "s/##_REVISION_##/$(shell svn info | grep Revision | sed "s/Revision: //")/" -e "s/##_DATE_##/$(shell date)/" -e "s/##_BUILD_##/$(shell ../bin/getversion.sh)/" -e "s/##_COMPUTER_##/$(COMPUTERNAME)/" -e "s/##_USER_##/$(USERNAME)/" Web/Portal/version.thtml > $@

%AssemblyVersionInfo.cs: AssemblyVersionInfo.cs
	cp $< $@

$(BUILDDIR):
	$(MKDIR) $(BUILDDIR)

.PHONY: debug
debug:
	@echo MODULES = $(MODULES)

#I would certainly like to have this one day...
#.PHONY: fxcop
#fxcop:
#	mkdir -p ../docs/FxCop
#	-../bin/FxCop/fxcopcmd /p:$(CLIENT_NAME).Dss.FxCop /out:../docs/FxCop/$(CLIENT_NAME).Dss.FxCop.xml

cleanlogs:
	rm -rf *.log
	rm -rf nvelocity.log.*

cleandtgbackups:
	find -iname '*cs~' -exec rm {} \;
	find -iname '*sql~' -exec rm {} \;
	find -iname '*orig' -exec rm {} \;
	find -iname '*mk~' -exec rm {} \;


reallyclean: clean cleanlogs cleandtgbackups

clean_release:
	$(RM) ../release/*	
	
regressall:
	find Test -name *Test.cs | sed -e s/.cs//g -e s/Test[/]/Cortside.Common.Test./ | xargs -i -t ../bin/nunit/nunit-console-x86.exe ../build/test/Cortside.Common.Test.dll -fixture:{} > regressall.txt

countlines:
	find -name '*cs' -print | xargs cat | wc -l
	find -name '*bas' -print | xargs cat | wc -l
	find -name '*cls' -print | xargs cat | wc -l
	find -name '*frm' -print | xargs cat | wc -l
	find -name '*asp' -print | xargs cat | wc -l
	find -name '*inc' -print | xargs cat | wc -l
	find -name '*js' -print | xargs cat | wc -l
	find -name '*aspx' -print | xargs cat | wc -l
	find -name '*sql' -print | xargs cat | wc -l
	find -name '*java' -print | xargs cat | wc -l
	find -name '*vm' -print | xargs cat | wc -l
	find -name '*vb' -print | xargs cat | wc -l
	find -name '*ascx' -print | xargs cat | wc -l
	find -name '*cshtml' -print | xargs cat | wc -l
	
style:
	find ../src -iname '*.cs' -exec ../bin/style.sh {} \;
	
uncrust:
	find ../src -iname '*.cs' -not -iname '*.designer.cs' -exec ../bin/uncrustify/uncrustify.exe -c ../bin/uncrustify/ul.cfg --no-backup {} \;
	
.PHONY: scriptorder
scriptorder:
	$(RM) ScriptOrder.txt combined_sql_scripts.sql
	$(RM) s.txt script.txt o.txt order.txt so.txt so1.txt svnlog.xml svnlog.csv
	#SVN
	svn log -r ${RELEASE_REVISION}:BASE --verbose --xml --stop-on-copy sql > svnlog.xml
	../bin/xml2csv.exe svnlog.xml svnlog.csv ",path" -N -S
	gawk -F"," "{print $2}" svnlog.csv | sed -e "s/,\/$(SVN_BRANCH_TAG)\/Dss\/src\///" | grep -v path > script.txt
	#ANY
	make -nB build_db > o.txt
	gawk '/sqlcmd/ {print $$14}' o.txt > order.txt
	-- for line in $$(cat order.txt); do grep $$line script.txt >> so.txt; done
	# strip out the blank lines 
	grep -v "^$$" so.txt > so1.txt
	# make sure the file has dos line endings,
	unix2dos -D so1.txt
	# strip off the last end of line
	awk 'NR > 1 { print h } { h = $$0 } END { ORS = ""; print h }' so1.txt | uniq > ScriptOrder.txt
	dos2unix ScriptOrder.txt
	$(RM) s.txt script.txt o.txt order.txt so.txt so1.txt svnlog.xml svnlog.csv

combine_sql_scripts:
	$(RM) combined_sql_scripts.sql
	-- for line in $$(cat ScriptOrder.txt); do printf '\n-- START OF: ' >> combined_sql_scripts.sql; echo $$line >> combined_sql_scripts.sql; printf '\n' >> combined_sql_scripts.sql; cat $$line >> combined_sql_scripts.sql; printf '\n-- END OF: ' >> combined_sql_scripts.sql; echo $$line >> combined_sql_scripts.sql; printf '\nGO\n' >> combined_sql_scripts.sql; done
	
package_sql: scriptorder combine_sql_scripts
	$(7ZIP) a $(RELEASEDIR)/Cortside.Common.sql-$(shell $(GETVERSION)).7z ScriptOrder.txt combined_sql_scripts.sql @ScriptOrder.txt
	$(RM) ScriptOrder.txt combined_sql_scripts.sql
	

package_cl:
	#../bin/svn2cl/svn2cl.sh -r ${RELEASE_REVISION}:BASE -o ../release/ChangeLog-$(shell $(GETVERSION)).txt

fixLineTermination:
	find . -type f -name '*.cs' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.as*x' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.css' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.js' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.vm' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.sql' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.html' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.cshtml' -print -exec unix2dos --safe \{\} \;
	find . -type f -name '*.config' -print -exec unix2dos --safe \{\} \;

takeownlib:
	../bin/makeitmine.cmd ../lib
	chmod -R +x $(LIBDIR)/*.dll
	chmod -R +x $(LIBDIR)/*.dll
	
takeown:
	chmod -R a+x ../bin/*
	chmod -R +x $(LIBDIR)/*.dll
	chmod -R +x $(LIBDIR)/*.dll
	chmod -R a+rw ../*
	@$(CHMOD) $(DTGDIR)/*.exe
	@$(CHMOD) $(DTGDIR)/*.dll
	@$(CHMOD) $(NANTDIR)/*.exe
	@$(CHMOD) $(NANTDIR)/*.dll

corflags:
	corflags ../bin/DTG/DataTierGenerator.exe \/32BIT+
				
# sql targets that don't yet exist	
clean_db:

build_db:

nuget:
	nuget pack Cortside.Common.Web.Hosting/Cortside.Common.Web.Hosting.csproj  -version $(shell $(GETVERSION))
	nuget push -Source http://nuget.Cortside.com -ApiKey 36fd337b-fbdb-47d6-be08-4eb497ee1470 Cortside.Common.Web.Hosting.$(shell $(GETVERSION)).nupkg
	nuget pack Cortside.Common.Configuration/Cortside.Common.Configuration.csproj  -version $(shell $(GETVERSION))
	nuget push -Source http://nuget.Cortside.com -ApiKey 36fd337b-fbdb-47d6-be08-4eb497ee1470 Cortside.Common.Configuration.$(shell $(GETVERSION)).nupkg
	rm -f *.nupkg
	