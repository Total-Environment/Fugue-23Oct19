import {
  mapStateToProps,
  mapDispatchToProps,
  __RewireAPI__ as ConnectorRewired
} from '../src/create_component_connector';
import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import {apiUrl} from '../src/helpers';

import 'babel-polyfill';
chai.use(sinonChai);

describe('CreateComponentConnector', () => {
  describe('mapStateToProps', () => {
    it('should return componentType, definition and masterData', () => {
      const state = {reducer:{
        genericDefinition: 1,
        masterData: 2,
        dependencyDefinitions: 1,
        componentCreateError: 1,
        componentAdding: true,
        assetDefinitions: 1,
      }};
      const props = {componentType: 'material'};
      const actual = mapStateToProps(state, props);
      const expected = {
        componentType: 'material',
        definitions: {},
        masterData: 2,
        dependencyDefinitions: 1,
        componentCreateError: 1,
        componentAdding: true,
        assetDefinitions: 1,
        dependencyDefinitionError: undefined
      };
      expect(actual).to.deep.equal(expected);
    });

    it('should return empty masterData if masterData is undefined', () => {
      const state = {reducer:{}};
      const props = {params: {componentType: 'material'}};
      const actual = mapStateToProps(state, props);
      expect(actual.dependencyDefinitions).to.deep.equal({});
    });

    it('should return empty dependencyData if dependencyData is undefined', () => {
      const state = {reducer:{}};
      const props = {params: {componentType: 'material'}};
      const actual = mapStateToProps(state, props);
      expect(actual.dependencyDefinitions).to.deep.equal({});
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onMaterialDefinitionFetch', () => {
      it('should call dispatch with MATERIAL_FETCH_SUCCEEDED when API returns data', async() => {
        let getStub = sinon.stub();
        getStub.withArgs(apiUrl('material-definitions/level 1'))
          .returns(Promise.resolve({data:'definition'}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMaterialDefinitionFetch("level 1");
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED',
          definition: 'definition'
        });
      });
      it('should call dispatch with MATERIAL_FETCH_FAILED when API returns error', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.reject('error'));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMaterialDefinitionFetch();
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED',
          error: 'error'
        });
      });
    });

    describe('onServiceDefinitionFetch', () => {
      it('should call dispatch with SERVICE_FETCH_SUCCEEDED when API returns data', async() => {
        let getStub = sinon.stub();
        getStub.withArgs(apiUrl('service-definitions/level 1'))
          .returns(Promise.resolve({data:'definition'}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onServiceDefinitionFetch("level 1");
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED',
          definition: 'definition'
        });
      });
      it('should call dispatch with SERVICE_FETCH_FAILED when API returns error', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.reject('error'));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onServiceDefinitionFetch();
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_SERVICE_DEFINITION_FETCH_FAILED',
          error: 'error'
        });
      });
    });

    describe('onMasterDataFetch', () => {
      it('should call dispatch with COMPONENT_MASTER_DATA_FETCH_SUCCEEDED when API returns data', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.resolve({data:'masterData'}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('0304');
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED',
          masterData: 'masterData'
        });
      });
      it('should call dispatch with COMPONENT_MASTER_DATA_FETCH_FAILED when API returns error', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.reject('error'));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('0304');
        expect(dispatchSpy).to.have.been.calledWith({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: 'error'});
      });
    });
    describe('onDependencyDefinitionFetch', () => {
      it('should call dispatch with DEPENDENCY_DEFINITION_FETCH_SUCCEEDED when API return data', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.resolve({data:'dependencyDefinition'}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED',
          dependencyDefinitionId: 'materialClassifications',
          dependencyDefinition: 'dependencyDefinition'
        });
      });
      it('should call dispatch when DEPENDENCY_DEFINITION_FETCH_FAILED when API return error', async() => {
        let getStub = sinon.stub();
        getStub.returns(Promise.reject({message:'error'}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.have.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId: 'materialClassifications',
          error: 'error'
        });
      });
    });

    describe('onAddMaterial', () => {
      let details;
      beforeEach(() => {
        details = {
          headers: [
            {
              columns: [
                {
                  key: 'image',
                  name: 'Image',
                  value: [0, 1, {name: 'MISI0002.png', id: "335355"}],
                  dataType: {
                    name: 'Array',
                    subType: {
                      name: 'StaticFile',
                      subType: null
                    }
                  }
                },
                {
                  name: 'HSN Code',
                  key: 'hsn_code',
                  value: 1234,
                  dataType: {
                    name: 'String',
                    subType: null
                  }
                }
              ],
              name: 'General',
              key: 'general'
            }
          ]
        };
        window.fileList = [[{name: "MISI0006.png", type: "image/png"}], [{name: "MISI0007.png", type: "image/png"}]];
      });
      it('should call post with arguments materials and redirect to material detail page if it returns successfully and dispatch ADD_MATERIAL_REQUESTED', async() => {
        let postStub = sinon.stub();
        postStub.withArgs(apiUrl('materials')).returns(Promise.resolve({data: {id: 1}}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddMaterial(details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'ADD_COMPONENT_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'ADD_COMPONENT_SUCCEEDED'});
        expect(pushSpy).to.be.calledWith('/materials/1');
      });
      it('should call post with arguments materials and show error dialog if it returns error', async() => {
        let postStub = sinon.stub();
        postStub.withArgs(apiUrl('materials')).returns(Promise.reject({response: {data: {message: 'error'}}}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddMaterial(details);
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_MATERIAL_FAILED', error: {message: 'error'}});
      });
    });

    describe('onAddService', () => {
      let details;
      beforeEach(() => {
        details = {
          headers: [
            {
              columns: [
                {
                  name: 'image',
                  key: 'Image',
                  value: [0, 1, {name: 'MISI0002.png', id: "335355"}],
                  dataType: {
                    name: 'Array',
                    subType: {
                      name: 'StaticFile',
                      subType: null
                    }
                  }
                },
                {
                  name: 'HSN Code',
                  key: 'hsn_code',
                  value: 1234,
                  dataType: {
                    name: 'String',
                    subType: null
                  }
                }
              ],
              name: 'General',
              key: 'general'
            }
          ]
        };
        window.fileList = [[{name: "MISI0006.png", type: "image/png"}], [{name: "MISI0007.png", type: "image/png"}]];
      });
      it('should call post with arguments services and redirect to service detail page if it returns successfully and dispatch ADD_SERVICE_REQUESTED', async() => {
        let postStub = sinon.stub();
        postStub.withArgs(apiUrl('services')).returns(Promise.resolve({data: {id: 1}}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddService(details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'ADD_COMPONENT_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'ADD_COMPONENT_SUCCEEDED'});
        expect(pushSpy).to.be.calledWith('/services/1');
      });
      it('should call post with arguments services and show error dialog if it returns error', async() => {
        let postStub = sinon.stub();
        postStub.withArgs(apiUrl('services')).returns(Promise.reject({response: {data: {message: 'error'}}}));
        ConnectorRewired.__Rewire__('axios', {post: postStub});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddService(details);
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_SERVICE_FAILED', error: {message: 'error'}});
      });
    });

    describe('onCancelMaterial', () => {
      it('should take the user to materials page', () => {
        const goBackSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {goBack: goBackSpy});
        mapDispatchToProps(() => {
        }).onCancelMaterial();
        expect(goBackSpy).called;
      });
    });

    describe('onCancelService', () => {
      it('should take the user to services page', () => {
        const goBackSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {goBack: goBackSpy});
        mapDispatchToProps(() => {
        }).onCancelService();
        expect(goBackSpy).called
      });
    });
  });
});
