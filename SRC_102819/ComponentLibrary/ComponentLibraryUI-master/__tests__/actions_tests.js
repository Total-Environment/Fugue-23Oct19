import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {onSfgFetchRequest, __RewireAPI__ as ActionsRewired} from "../src/actions";

chai.use(sinonChai);

describe('actions', () => {
  describe('onSfgFetchRequest', () => {
    let dispatchSpy;
    beforeEach(() => {
      dispatchSpy = sinon.spy();
    });

    it('should dispatch SFG_FETCH_SUCCEEDED when sfg exists', async () => {
      ActionsRewired.__Rewire__('axios', {
        get: sinon.stub().returns(Promise.resolve({data: 1}))
      });
      await onSfgFetchRequest('SFG0001')(dispatchSpy);
      expect(dispatchSpy).to.be.calledWith({
        type: 'SFG_FETCH_SUCCEEDED',
        details: 1,
      });
    });


    it('should dispatch SFG_FETCH_FAILED when sfg does not exists', async () => {
      ActionsRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 404}}))});

      await onSfgFetchRequest('SFG0001')(dispatchSpy);
      expect(dispatchSpy).to.be.calledWith({
        type: 'SFG_FETCH_FAILED',
        code: 'SFG0001',
        error: { type: 'NotFound', message: 'SFG SFG0001 is not available in the database' }
      });
    });

    it('should dispatch SFG_FETCH_FAILED when sfg does not exists', async () => {
      ActionsRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 500}}))});

      await onSfgFetchRequest('SFG0001')(dispatchSpy);
      expect(dispatchSpy).to.be.calledWith({
        type: 'SFG_FETCH_FAILED',
        code: 'SFG0001',
        error: { type: 'Unknown', message: 'Some thing went wrong. Problem has been recorded and will be fixed.' }
      });
    });
  });
});
