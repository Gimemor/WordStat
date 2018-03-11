#!/usr/bin/env python3
import sys
import xml.etree.cElementTree as etree
from enum import Enum
xml_path = 'annot.opcorpora.xml'


START_EVENT = 'start'
END_EVENT = 'end'
START_NS_EVENT = 'start-ns'
ELEMENT_TOKEN = 'token'
ELEMENT_TEXT  = 'text'
ELEMENT_ANNOTATION = 'annotation'
ELEMENT_TAGS = 'tags'
ELEMENT_PARAGRAPHS = 'paragraphs'
ELEMENT_PARAGRAPH = 'paragraph'
ELEMENT_SENTENCE = 'sentence'
ELEMENT_SENTENCE_SOURCE = 'source'
ELEMENT_TOKENS = 'tokens'
ELEMENT_TOKEN = 'token'
ELEMENT_TFR = 'tfr'
ELEMENT_V = 'v'
ELEMENT_L = 'l'
ELEMENT_G = 'g'

class ParserState(Enum):
    WAITING_TEXT = 0
    READING_TEXT = 1
    READING_TAGS = 2
    READING_PARAGRAPHS = 3
    READING_PARAGRAPH = 4
    READING_SENTENCE = 5
    READING_SENTENCE_SOURCE = 6
    READING_TOKENS = 7
    READING_TOKEN = 8
    READING_TFR = 9
    READING_V = 10
    READING_L = 11
    READING_G = 12

class OpenCorporaParser:
    nsmap = {}

    def __init__(self):
        self.state = [ ParserState.WAITING_TEXT ]

    state_selector = {
        ParserState.WAITING_TEXT:
            lambda self, element, event: self.process_waiting_text(event, element),
        ParserState.READING_TEXT:
            lambda self, element, event: self.process_reading_text(event, element),
        ParserState.READING_TAGS:
            lambda self, element, event: self.process_reading_tags(event, element),
        ParserState.READING_PARAGRAPHS:
            lambda self, element, event: self.process_reading_paragraphs(event, element),
        ParserState.READING_PARAGRAPH:
            lambda self, element, event: self.process_reading_paragraph(event, element),
        ParserState.READING_SENTENCE:
            lambda self, element, event: self.process_reading_sentence(event, element),
        ParserState.READING_SENTENCE_SOURCE:
            lambda self, element, event: self.process_sentence_source(event, element),
        ParserState.READING_TOKEN:
            lambda self, element, event: self.process_reading_token(event, element),
        ParserState.READING_TOKENS:
            lambda self, element, event: self.process_reading_tokens(event, element),
        ParserState.READING_TFR:
            lambda self, element, event: self.process_reading_tfr(event, element),
        ParserState.READING_V:
            lambda self, element, event: self.process_reading_v(event, element),
        ParserState.READING_L:
            lambda self, element, event: self.process_reading_l(event, element),
        ParserState.READING_G:
            lambda self, element, event: self.process_reading_g(event, element),
    }

    def process_reading_g(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_G:
            self.state.pop()
            element.clear()

    def process_reading_l(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_L:
            self.state.pop()
            element.clear()
        elif event == START_EVENT and element.tag == ELEMENT_G:
            self.state.append(ParserState.READING_G)
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            тэг в параграфе не ожидался {}
                            '''.format(element))

    def process_reading_v(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_V:
            self.state.pop()
            element.clear()
        elif event == START_EVENT and element.tag == ELEMENT_L:
            self.state.append(ParserState.READING_L)
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            тэг в параграфе не ожидался {}
                            '''.format(element))

    def process_reading_tfr(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_TFR:
            self.state.pop()
            element.clear()
            return;
        elif event == START_EVENT and element.tag == ELEMENT_V:
            self.state.append(ParserState.READING_V)
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            тэг в параграфе не ожидался {}
                            '''.format(element))
    def process_reading_tags(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_TAGS:
            self.state.pop()
            element.clear()
            return

    def process_sentence_source(self, event, element):
        print('\tИсточник: {}'.format(element.text))
        print(element.text, file=sys.stderr)
        if event == END_EVENT and element.tag == ELEMENT_SENTENCE_SOURCE:
            self.state.pop()
            element.clear()
            return

    def process_reading_token(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_TOKEN:
            self.state.pop()
            element.clear()
            return
        elif event == START_EVENT and element.tag == ELEMENT_TFR:
            self.state.append(ParserState.READING_TFR)
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            тэг в параграфе не ожидался {}
                            '''.format(element))

    def process_reading_tokens(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_TOKENS:
            self.state.pop()
            element.clear()
            return
        elif event == START_EVENT and element.tag == ELEMENT_TOKEN:
            attrib = element.attrib
            print('\tЧитаем токен {}'.format(attrib['id']))
            self.state.append(ParserState.READING_TOKEN)
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            тэг в параграфе не ожидался {}
                            '''.format(element))

    def process_reading_sentence(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_SENTENCE:
            self.state.pop()
            element.clear()
            return
        elif event == END_EVENT:
            raise Exception('''
                            Ошибка в структуре документа,
                            закрывающий тэг в параграфе не ожидался {}
                            '''.format(element))
        if element.tag == ELEMENT_SENTENCE_SOURCE:
            self.state.append(ParserState.READING_SENTENCE_SOURCE)
        elif element.tag == ELEMENT_TOKENS:
            self.state.append(ParserState.READING_TOKENS)
        else:
            print(self.state)
            raise Exception('''
                            Ошибка в структуре документа,
                            открывающий тэг в параграфе не ожидался {}
                            '''.format(element))

    def process_reading_paragraph(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_PARAGRAPH:
            self.state.pop()
            element.clear()
            return
        elif event == END_EVENT:
            raise Exception('''
                            Ошибка в структуре документа,
                            закрывающий тэг в параграфе не ожидался {}
                            '''.format(element))

        if element.tag == ELEMENT_SENTENCE:
            self.state.append(ParserState.READING_SENTENCE)

    def process_reading_paragraphs(self, event, element):
        if event == END_EVENT and element.tag == ELEMENT_PARAGRAPHS:
            self.state.pop()
            element.clear()
            return
        elif event == END_EVENT:
            raise Exception('''
                            Ошибка в структуре документа,
                            закрывающий тэг в параграфе не ожидался {}
                            '''.format(element))
        if element.tag == ELEMENT_PARAGRAPH:
            attribs = element.attrib
            print('Читаем параграф {}'.format(attribs['id'] if 'id' in attribs else ''))
            self.state.append(ParserState.READING_PARAGRAPH)

    def process_reading_text(self, event, element):
        if event != START_EVENT and event != END_EVENT:
            return
        if event == START_EVENT:
            if element.tag == ELEMENT_TAGS:
                self.state.append(ParserState.READING_TAGS)
            elif element.tag == ELEMENT_PARAGRAPHS:
                print('/***/', file = sys.stderr)
                self.state.append(ParserState.READING_PARAGRAPHS)
            else:
                raise Exception('Ошибка при разборе текста, проверяй: {}'.format(element))
        elif event == END_EVENT and element.tag == ELEMENT_TEXT:
            self.state.pop()
            element.clear()
        else:
            raise Exception('''
                            Ошибка в структуре документа,
                            элемент завершения несоответствуют состоянию
                            либо неожиданный элемент открывается:\n{}'''.format(element.tag))



    def process_waiting_text(self, event, element):
        if event != START_EVENT:
            return
        if element.tag == ELEMENT_TEXT:
           self.state.append(ParserState.READING_TEXT)
        elif element.tag == ELEMENT_ANNOTATION:
            # читаем тэги
            print(element.attrib)
        else:
            raise Exception('Ошибка формата при ожидании текста, проверяй')

    def parse(self, xml_path):
        with open(xml_path, 'r') as f:
            iterparser = etree.iterparse(f, events=(START_EVENT, START_NS_EVENT, END_EVENT))
            for (event, elem) in iterparser:
                OpenCorporaParser.state_selector[self.state[-1]](self, elem, event)

if sys.argv[0] == __file__:
    parser = OpenCorporaParser()
    parser.parse(xml_path)
'''
    for (event, elem) in etree.iterparse(xml_path, events=('start', 'end', 'start-ns', 'end-ns')):
      if(elem.tag == 'token'):
        print(event, elem)
'''
