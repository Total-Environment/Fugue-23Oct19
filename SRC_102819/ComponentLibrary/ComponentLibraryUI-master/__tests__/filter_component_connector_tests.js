import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import axios from 'axios';
import {
  mapDispatchToProps,
  mapStateToProps,
  __RewireAPI__ as ConnectorRewired
} from '../src/filter_component_connector';

chai.use(sinonChai);

describe('filter component connector', () => {
  describe('map state to props', () => {
    it('should return definition when state contain definitions', () => {
      const state = {reducer: {definitions: {flooring: 'flooring definition'},brandDefinitions:{brand: 'brand definition'}}};
      const props = {onClose: 'close', componentType: 'material', group: 'clay', onApply: 'apply',
        onClear: 'clear',modifiedDefinition:'modified',onCancelErrorDialog:'cancel error'};
      const expected = {
        definitions: {flooring: 'flooring definition'},
        brandDefinitions: {brand: 'brand definition'},
        onClose: 'close',
        componentType: 'material',
        group: 'clay',
        onApply: 'apply',
        onClear: 'clear',
        modifiedDefinition:'modified',
        errorInFilter:undefined,
        onCancelErrorDialog: 'cancel error',
        dependencyDefinitions: undefined,
        levels: undefined
      };
      const actual = mapStateToProps(state, props);
      expect(actual).to.deep.equal(expected);
    });

    it('should return empty object when state does not definitions', () => {
      const state = {reducer: {}};
      const props = {onClose: 'close', componentType: 'material',
        group: 'clay', onApply: 'apply',onClear:'clear',onCancelErrorDialog:'cancel error'};
      const expected = {definitions: {},brandDefinitions:{}, onClose: 'close', componentType: 'material',
        dependencyDefinitions: undefined,levels: undefined,
        group: 'clay', onApply: 'apply',onClear:'clear',modifiedDefinition:undefined,errorInFilter:undefined,onCancelErrorDialog:'cancel error'};
      const actual = mapStateToProps(state, props);
      expect(actual).to.deep.equal(expected);
    });
  });
  describe('map dispatch to props', () => {
    let dispatchSpy, returnedObject;
    beforeEach(() => {
      dispatchSpy = sinon.spy();
      returnedObject = mapDispatchToProps(dispatchSpy);
    });
    describe('onMaterialDefinitionFetch', () => {
      it('should dispatch COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED when material definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'material Definition'}))});
        await returnedObject.onMaterialDefinitionFetch('clay material');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED',
          definition: 'material Definition'
        });
      });

      it('should dispatch COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED when api returns error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject('error'))});
        await mapDispatchToProps(dispatchSpy).onMaterialDefinitionFetch('clay material');
        await returnedObject.onMaterialDefinitionFetch('clay material');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED',
          error: 'error'
        });
      });
    });


    describe('onBrandDefinitionFetch',()=>{
      it('should dispatch COMPONENT_BRAND_DEFINITION_FETCH_SUCEEDED when brand definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'brand Definition'}))});
        await returnedObject.onBrandDefinitionFetch();
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED',
          brandDefinition: 'brand Definition'
        });
      });

      it('should dispatch COMPONENT_BRAND_DEFINITION_FETCH_FAILED when api returns error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject('error'))});
        await mapDispatchToProps(dispatchSpy).onBrandDefinitionFetch();
        await returnedObject.onBrandDefinitionFetch(  );
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_BRAND_DEFINITION_FETCH_FAILED',
          error: 'error'
        });
      });
    });

    describe('onServiceDefinitionFetch', () => {
      it('should dispatch COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED when service definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'service Definition'}))});
        await returnedObject.onServiceDefinitionFetch('flooring dado counter');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED',
          definition: 'service Definition'
        });
      });
      it('should dispatch COMPONENT_SERVICE_DEFINITION_FETCH_FAILED when api returns error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject('error'))});
        await returnedObject.onServiceDefinitionFetch('flooring dado counter');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_SERVICE_DEFINITION_FETCH_FAILED',
          error: 'error'
        });
      });
    });

    describe('onPackageDefinitionFetch', () => {
      it('should dispatch PACKAGE_DEFINITION_FETCH_SUCCEEDED when package definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'package Definition'}))});
        await returnedObject.onPackageDefinitionFetch('package definition');
        expect(dispatchSpy).to.be.calledWith({
          type: 'PACKAGE_DEFINITION_FETCH_SUCCEEDED',
          definition: 'package Definition'
        });
      });

      it('should dispatch PACKAGE_DEFINITION_FETCH_FAILED when api returns error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject('error'))});
        await returnedObject.onPackageDefinitionFetch('package');
        expect(dispatchSpy).to.be.calledWith({
          type: 'PACKAGE_DEFINITION_FETCH_FAILED',
          error: 'error'
        });

      });
    });


  });
});

