import opencorpora
corpus = opencorpora.CorpusReader('annot.opcorpora.xml')
for i in corpus.iter_documents():
    print(i)
