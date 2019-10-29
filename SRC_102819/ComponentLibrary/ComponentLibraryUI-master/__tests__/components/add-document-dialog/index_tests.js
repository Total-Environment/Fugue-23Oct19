import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {
  AddDocumentDialog,
  SelectFromLocalSystem,
  SearchInFileDirectory,
  ComponentDocuments,
  ComponentDocument,
  Document,
  __RewireAPI__ as AddDocumentDialogRewired
} from '../../../src/components/add-document-dialog';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Add Document Dialog', () => {
  let onLinkSpy, onPageNumberChangeSpy, onOkSpy, onCancelSpy, getMaterialDocumentsByGroupAndColumnNameSpy,
    destroyMaterialDocumentsSpy, props;
  beforeEach(() => {
    onLinkSpy = sinon.spy();
    onPageNumberChangeSpy = sinon.spy();
    onOkSpy = sinon.spy();
    onCancelSpy = sinon.spy();
    getMaterialDocumentsByGroupAndColumnNameSpy = sinon.spy();
    destroyMaterialDocumentsSpy = sinon.spy();
    props = {
      componentType: "material",
      group: "test group",
      columnName: "image",
      data: {
        values: {
          items: [
            {
              materialCode: "test material code",
              shortDescription: "test short description",
              documentDtos: [{url: "test url", name: "test name", id: "test id"}]
            }
          ],
          pageNumber: 1,
          recordCount: 1,
          batchSize: 1
        }
      },
      showLocalSystem: true,
      getMaterialDocumentsByGroupAndColumnName: getMaterialDocumentsByGroupAndColumnNameSpy,
      destroyMaterialDocuments: destroyMaterialDocumentsSpy,
      onLink: onLinkSpy,
      onPageNumberChange: onPageNumberChangeSpy,
      onOk: onOkSpy,
      onCancel: onCancelSpy
    };
  });

  describe('render', () => {
    it('should render', () => {
      const wrapper = shallow(<AddDocumentDialog {...props} />);
      expect(wrapper).to.have.descendants('Modal');
      expect(wrapper).to.have.descendants('SelectFromLocalSystem');
      expect(wrapper).to.have.descendants('SearchInFileDirectory');
      expect(wrapper).to.have.descendants('ComponentDocuments');
      expect(wrapper.find('button').at(0)).to.have.text('Cancel');
      expect(wrapper.find('button').at(1)).to.have.text('OK');
    });

    it('should call onCancel when Cancel button is clicked', () => {
      const preventDefaultSpy = sinon.spy();
      const wrapper = shallow(<AddDocumentDialog {...props} />);
      wrapper.find('button').at(0).simulate('click', {preventDefault: preventDefaultSpy});
      expect(preventDefaultSpy).to.have.been.called;
      expect(onCancelSpy).to.have.been.called;
    });

    it('should call onOk when Ok button is clicked', () => {
      const preventDefaultSpy = sinon.spy();
      const wrapper = shallow(<AddDocumentDialog {...props} />);
      wrapper.find('button').at(1).simulate('click', {preventDefault: preventDefaultSpy});
      expect(preventDefaultSpy).to.have.been.called;
      expect(onOkSpy).to.have.been.called;
    });
  });

  describe('componentDidMount', () => {
    it('should call getMaterialDocumentsByGroupAndColumnName', () => {
      const wrapper = shallow(<AddDocumentDialog {...props} />);
      wrapper.instance().componentDidMount();
      expect(getMaterialDocumentsByGroupAndColumnNameSpy).to.be.calledWith("test group", "image", 1);
    });
  });

  describe('componentWillUnmount', () => {
    it('should call destroyMaterialDocuments', () => {
      const wrapper = shallow(<AddDocumentDialog {...props} />);
      wrapper.instance().componentWillUnmount();
      expect(destroyMaterialDocumentsSpy).to.be.calledWith();
    });
  });
});

describe('Select From Local System', () => {
  describe('render', () => {
    it('should render input and button', () => {
      const wrapper = shallow(<SelectFromLocalSystem />);
      expect(wrapper).to.have.descendants('input');
      expect(wrapper).to.have.descendants('button');
    });
  });
});

describe('Search In File Directory', () => {
  describe('render', () => {
    it('should render form with input and button', () => {
      const wrapper = shallow(<SearchInFileDirectory componentType="material" />);
      expect(wrapper).to.have.descendants('form');
      expect(wrapper).to.have.descendants('input');
      expect(wrapper).to.have.descendants('button');
    });

    it('should not call onSearch when button is clicked and the keyword length is less than 2', () => {
      const onSearchSpy = sinon.spy();
      const preventDefaultSpy = sinon.spy();
      const wrapper = shallow(<SearchInFileDirectory componentType="material" onSearch={onSearchSpy} />);
      wrapper.find('button').simulate('click', {preventDefault: preventDefaultSpy});
      expect(preventDefaultSpy).to.have.been.called;
      expect(onSearchSpy).to.have.not.been.called;
    });

    it('should call onSearch when the button is clicked and keyword length is > 2', () => {
      const onSearchSpy = sinon.spy();
      const preventDefaultSpy = sinon.spy();
      const wrapper = shallow(<SearchInFileDirectory componentType="material" onSearch={onSearchSpy} />);
      wrapper.find('input').simulate('change', {
        target: {
          value: "123123"
        }
      });
      wrapper.find('button').simulate('click', {preventDefault: preventDefaultSpy});
      expect(preventDefaultSpy).to.have.been.called;
      expect(onSearchSpy).to.have.been.called;
    });
  });
});

describe('Component Documents', () => {
  describe('render', () => {
    it('should render Loading when results are not available and fetching', () => {
      const wrapper = shallow(<ComponentDocuments />);
      expect(wrapper).to.have.descendants('Loading');
    });

    it('should render data when results are available and not fetching', () => {
      const onLinkSpy = sinon.spy();
      const onPageNumberChangeSpy = sinon.spy();
      const data = {
        values: {
          items: [
            {
              materialCode: "test material code",
              shortDescription: "test short description",
              documentDtos: [{url: "test url", name: "test name", id: "test id"}]
            }
          ],
          pageNumber: 1,
          recordCount: 1,
          batchSize: 1
        }
      };
      const wrapper = shallow(<ComponentDocuments componentType="material" isFetching={false} data={data}
                                                  onLink={onLinkSpy} onPageNumberChange={onPageNumberChangeSpy}/>);
      expect(wrapper).to.have.descendants('table');
      expect(wrapper).to.have.descendants('Pagination');
      expect(wrapper.find('ComponentDocument')).to.have.length(1);
    });
  });
});

describe('Component Document', () => {
  describe('render', () => {
    it('should render a tr', () => {
      const onLinkSpy = sinon.spy();
      const document = {url: "test url", name: "test name", id: "test id"};
      const documents = [document];
      const linkFile = {document: {url: "link url", name: "link name", id: "link id"}};
      const wrapper = shallow(<ComponentDocument componentCode="test component code"
                                                 shortDescription="test short description" documents={documents}
                                                 linkFile={linkFile} onLink={onLinkSpy}/>);
      expect(wrapper).to.have.descendants('tr');
      expect(wrapper.find('td').at(0)).to.have.text('test short description');
      expect(wrapper.find('td').at(1)).to.have.text('test component code');
      expect(wrapper.find('Document')).to.have.length(1);
    });
  });
});

describe('Document', () => {
  describe('render', () => {
    it('should render a anchor and link button with text as Link', () => {
      const onLinkSpy = sinon.spy();
      const document = {url: "test url", name: "test name", id: "test id"};
      const linkFile = {document: {url: "link url", name: "link name", id: "link id"}};
      const wrapper = shallow(<Document document={document} linkFile={linkFile} onLink={onLinkSpy}/>);
      expect(wrapper).to.have.descendants('div');
      expect(wrapper.find('a')).to.have.text('test name');
      expect(wrapper.find('button')).to.have.text('Link');
    });

    it('should render a anchor and link button with text as Linked', () => {
      const onLinkSpy = sinon.spy();
      const document = {url: "test url", name: "test name", id: "test id"};
      const linkFile = {document: {url: "test url", name: "test name", id: "test id"}};
      const wrapper = shallow(<Document document={document} linkFile={linkFile} onLink={onLinkSpy}/>);
      expect(wrapper).to.have.descendants('div');
      expect(wrapper.find('a')).to.have.text('test name');
      expect(wrapper.find('button')).to.have.text('Linked');
    });

    it('should call onLink when button is clicked', () => {
      const onLinkSpy = sinon.spy();
      const preventDefaultSpy = sinon.spy();
      const document = {url: "test url", name: "test name", id: "test id"};
      const linkFile = {document: {url: "link url", name: "link name", id: "link id"}};
      const wrapper = shallow(<Document componentCode="test component code" document={document} linkFile={linkFile}
                                        onLink={onLinkSpy}/>);
      wrapper.find('button').simulate('click', {preventDefault: preventDefaultSpy});
      expect(preventDefaultSpy).to.have.been.called;
      expect(onLinkSpy).to.have.been.called;
    });
  });
});
