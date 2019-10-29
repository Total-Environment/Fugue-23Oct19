import {mapStateToProps, mapDispatchToProps,__RewireAPI__ as ConnectorRewired } from '../src/checklist_connector';
import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('ChecklistConnector', () => {
  describe('mapStateToProps', () => {
    it('mapStateToProps should return checklistId and checklistDetails', () => {
        const checklistId = 'SRID0004';
        const state = {reducer:{currentChecklistDetails: {template: 'QEM', name: 'my checklist'}, error:'error' }};
        const props = {params: {checklistId}};
        const expectedDetails = mapStateToProps(state, props);
        expect(expectedDetails).to.deep.equal({
            checklistId,
            checklistDetails: state.reducer.currentChecklistDetails,
            error: state.reducer.error
        });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onChecklistFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch CHECKLIST_FETCH_SUCCEEDED when checklist exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data:{content: {entries: [{cells: [{value: "S.No"}]}]}}}))});
        await returnedObject.onChecklistFetchRequest('SRID0004');
        expect(dispatchSpy).to.be.calledWith({
          type: 'CHECKLIST_FETCH_SUCCEEDED',
          checklist: { content: { entries: [{ cells: [{ key: "0 0", value: "S.No" }], key: 0 }] } }});
      });

      it('should dispatch CHECKLIST_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({message:'Checklist not found'}))});
        await returnedObject.onChecklistFetchRequest('SRID0004');
        expect(dispatchSpy).to.be.calledWith({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Checklist not found'
        });
      });

      it('should dispatch CHECKLIST_FETCH_FAILED when response status is 500',async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status:500}}))});
        await returnedObject.onChecklistFetchRequest('SRID0004');
        expect(dispatchSpy).to.be.calledWith({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch CHECKLIST_FETCH_FAILED when response status is 404',async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status:404}}))});
        await returnedObject.onChecklistFetchRequest('SRID0004');
        expect(dispatchSpy).to.be.calledWith({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Checklist is not found'
        });
      });

      it('should dispatch CHECKLIST_FETCH_FAILED when response status is 403',async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status:403}}))});
        await returnedObject.onChecklistFetchRequest('SRID0004');
        expect(dispatchSpy).to.be.calledWith({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });
    });
  });
});
