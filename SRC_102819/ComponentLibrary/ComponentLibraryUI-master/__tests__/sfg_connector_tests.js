import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/sfg_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('sfg Connector', () => {
  describe('mapStateToProps', () => {
    it('should return current sfg details with sfgCode as parameter', () => {
      const state = {
        reducer: {
          components: {"SFG0001": 2},
          sfgCosts: {'SFG0001': 'sfg costs'},
          error: '',
          classificationDefinition: {"Classification definition": "Classification definition"}
        }
      };
      const props = {params: {sfgCode: 'SFG0001'}};
      const actual = mapStateToProps(state, props);
      const expected = {
        sfgCode: 'SFG0001',
        details: 2,
        sfgCostError: undefined,
        cost: 'sfg costs',
        error: '',
        sfgError: undefined
      };
      expect(actual).to.deep.equal(expected);
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onSfgCostFetchRequest', () => {
      let dispatchSpy, returnedObject, props;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        props = {params: {sfgCode: 'SFG0001'}};
        returnedObject = mapDispatchToProps(dispatchSpy, props);
      });

      it('should dispatch SFG_COST_FETCH_SUCCEEDED when sfg cost exists', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub().returns(Promise.resolve({
            data: {
              cost: 'cost'
            }
          }))
        });
        await returnedObject.onSfgCostFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'SFG_COST_FETCH_SUCCEEDED',
          cost: {cost: "cost"},
          sfgCode: 'SFG0001'
        });
      });


      it('should dispatch SFG_COST_FETCH_FAILED when sfg does not exists', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub().returns(Promise.reject({
            response: {
              status: 404,
              data: 'error'
            }
          }))
        });

        await returnedObject.onSfgCostFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'SFG_COST_FETCH_FAILED',
          error: 'error'
        });
      });

      it('should dispatch SFG_COST_FETCH_FAILED when sfg does not exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 500}}))});

        await returnedObject.onSfgCostFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'SFG_COST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch SFG_COST_FETCH_FAILED when sfg does not exists', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub().returns(Promise.reject({
            response: {
              status: 400,
              data: {message: 'error'}
            }
          }))
        });

        await returnedObject.onSfgCostFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'SFG_COST_FETCH_FAILED',
          error: 'error'
        });
      });
    });
    describe('onSfgDestroy', () => {
      it('should dispatch SFG_DESTROY', async () => {
        const dispatchSpy = sinon.spy();
        const props = {params: {sfgCode: 'SFG0001'}};
        const returnedObject = mapDispatchToProps(dispatchSpy, props);
        await returnedObject.onSfgDestroy();
        expect(dispatchSpy).to.be.calledWith({
          type: 'SFG_DESTROY'
        });
      });
    });

  });
});
