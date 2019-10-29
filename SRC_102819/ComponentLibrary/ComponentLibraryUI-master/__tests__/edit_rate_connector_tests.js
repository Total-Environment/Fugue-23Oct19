import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/edit_rate_connector';
import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import axios from 'axios';

chai.use(sinonChai);

describe('Edit Rate Connector', () => {
  describe('mapStateToProps', () => {
    it('should returns material rates when passed props with state', () => {
      const state = {reducer: {rates: {material: "material"}}};
      const props = {};
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal({
        rates: "material"
      });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onRateFetchRequest', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch RATES_FETCH_SUCCEEDED when rates exist', async () => {
        let postStub = sinon.stub();
        postStub.returns(Promise.resolve({data: 'rates'}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onRateFetchRequest();
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'RATES_FETCH_SUCCEEDED',
          rates: 'rates',
          componentType: 'material'
        });
      });

      it('should dispatch RATES_FETCH_FAILED api throws an error', async () => {
        let postStub = sinon.stub();
        postStub.returns(Promise.reject({data: 'error', message: 'error has occurred'}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onRateFetchRequest();
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'RATES_FETCH_FAILED',
          ratesError: 'error has occurred',
          componentType: 'material'
        });
      });
    });
  });
});
