#!/usr/bin/env python3
from __future__ import unicode_literals
import sys
import spacy.lang.ru
import re
from spacy.lang.ru import Russian
import spacy;
corpora_path = 'raw_corpora.txt'

global_word_count = {}
texts_word_count = []

nlp = Russian()
tokenizer = nlp.Defaults.create_tokenizer()
with open(corpora_path, 'r') as f:
    for cnt, line in enumerate(f):
        line = line.strip()
        if line == '/***/':
            texts_word_count.append({})
            continue
        doc = nlp(line)
        for word in doc:
            if re.match('[\w]+', word.text, re.I) == None:
                continue
            if not word.text in global_word_count:
                global_word_count[word.text] = 1
            else:
                global_word_count[word.text] += 1

            if not word.text in texts_word_count[-1]:
                texts_word_count[-1][word.text] = 1
            else:
                texts_word_count[-1][word.text] += 1

for word in global_word_count:
    doc_cnt = 0
    for text in texts_word_count:
        if word in text:
            doc_cnt += 1
    print('{}\t{}\t{}'.format(global_word_count[word], doc_cnt, word))

