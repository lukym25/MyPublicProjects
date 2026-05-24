PAGES= \
	index \
	rules \
	score

PAGES_HTML=$(addprefix out/, $(addsuffix .html, $(PAGES)))

all: $(PAGES_HTML) out/main.css out/rules.pdf

.PHONY: all clean upload

clean:
	rm -rf out/* tmp/ 

out/%.html: %.md template.html
	pandoc --template template.html -o $@ $<

out/rules.pdf: tmp/rules.odt
	soffice --headless --convert-to pdf tmp/rules.odt --outdir out/

tmp/rules.odt: rules.md
	mkdir -p tmp/
	pandoc -o tmp/rules.odt rules.md

out/score.html: score.csv table.py
	./table.py <score.csv | pandoc --template template.html --metadata title="Score" - >out/score.html

out/main.css: main.css
	cp main.css out/

upload:
	scp out/* marislel@u-pl1.ms.mff.cuni.cz:WWW/make/
