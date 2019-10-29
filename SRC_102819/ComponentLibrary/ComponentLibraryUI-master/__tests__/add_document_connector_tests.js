import { mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired } from '../src/add_document_connector';
import chai, { expect } from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('AddDocumentConnector', () => {
  describe('mapStateToProps', () => {
    it('mapStateToProps should return AddDocumentDialog with correct properties', () => {
      const state = {
        addDocumentReducer: {
          data: 'data',
          error: 'error',
          isFetching: false
        }
      };
      const onOkSpy = sinon.spy();
      const onCancelSpy = sinon.spy();
      const props = {
        componentType: 'componentType',
        group: 'group',
        columnName: 'columnName',
        showLocalSystem: true,
        onOk: onOkSpy,
        onCancel: onCancelSpy,
        columnHeader: 'columnHeader'
      };
      const expectedDetails = mapStateToProps(state, props);
      expect(expectedDetails).to.deep.equal({
        componentType: 'componentType',
        group: 'group',
        columnName: 'columnName',
        showLocalSystem: true,
        data: 'data',
        error: 'error',
        isFetching: false,
        onOk: onOkSpy,
        onCancel: onCancelSpy,
        columnHeader: 'columnHeader'
      });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('getMaterialDocumentsByGroupAndColumnName', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED when material documents exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'materialDocuments' })) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED',
          details: 'materialDocuments'
        });
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED when response status is 400', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ response: { status: 400 } })) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
          error: { type: 'BadRequest' }
        });
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject('error')) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
          error: { type: 'Unknown', error: 'error' }
        });
      });
    });

    describe('getMaterialDocumentsByGroupAndColumnNameAndKeyWord', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED when material documents exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'materialDocuments' })) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED',
          details: 'materialDocuments'
        });
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED when response status is 400', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ response: { status: 400 } })) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'BadRequest' }
        });
      });

      it('should dispatch GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject('error')) });
        await returnedObject.getMaterialDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'Unknown', error: 'error' }
        });
      });
    });

    describe('getServiceDocumentsByGroupAndColumnName', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED when material documents exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'materialDocuments' })) });
        await returnedObject.getServiceDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED',
          details: 'materialDocuments'
        });
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED when response status is 400', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ response: { status: 400 } })) });
        await returnedObject.getServiceDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
          error: { type: 'BadRequest' }
        });
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject('error')) });
        await returnedObject.getServiceDocumentsByGroupAndColumnName('group', 'columnName');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED',
          error: { type: 'Unknown', error: 'error' }
        });
      });
    });

    describe('getServiceDocumentsByGroupAndColumnNameAndKeyWord', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED when material documents exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'materialDocuments' })) });
        await returnedObject.getServiceDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED',
          details: 'materialDocuments'
        });
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED when response status is 400', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ response: { status: 400 } })) });
        await returnedObject.getServiceDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'BadRequest' }
        });
      });

      it('should dispatch GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject('error')) });
        await returnedObject.getServiceDocumentsByGroupAndColumnNameAndKeyWord('group', 'columnName', 'keyWord');
        expect(dispatchSpy).to.be.calledWith({
          type: 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED',
          error: { type: 'Unknown', error: 'error' }
        });
      });
    });

    describe('destroyMaterialDocuments', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch DESTROY_MATERIAL_DOCUMENTS', async () => {
        await returnedObject.destroyMaterialDocuments();
        expect(dispatchSpy).to.be.calledWith({
          type: 'DESTROY_MATERIAL_DOCUMENTS'
        });
      });

      it('should dispatch DESTROY_SERVICE_DOCUMENTS', async () => {
        await returnedObject.destroyServiceDocuments();
        expect(dispatchSpy).to.be.calledWith({
          type: 'DESTROY_SERVICE_DOCUMENTS'
        });
      });
    });
  });
});
