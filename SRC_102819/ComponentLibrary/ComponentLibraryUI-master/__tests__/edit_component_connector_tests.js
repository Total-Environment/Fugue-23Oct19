import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/edit_component_connector';
import {apiUrl} from '../src/helpers';
import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import axios from 'axios';

chai.use(sinonChai);

describe('EditComponentConnector', () => {
  describe('mapStateToProps', () => {
    it('should return dependency definition and material details', () => {
      const state = {
        reducer: {
          dependencyDefinitions: {name: 'dependency definition'},
          assetDefinitions: 1,
          components: {'IRN000001': {}},
          dependencyDefinitionError: {material: undefined, service: undefined}
        }
      };
      const props = {route: {componentType: 'material'}, params: {componentCode: 'IRN000001'}};
      const actual = mapStateToProps(state, props);
      const expected =
        {
          componentCode: 'IRN000001',
          componentType: 'material',
          dependencyDefinitions: {name: 'dependency definition'},
          details: {},
          masterData: {},
          componentUpdating: false,
          componentUpdateError: undefined,
          assetDefinitions: 1,
          error: false
        };
      expect(actual).to.deep.equal(expected);
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onMaterialFetchRequest', () => {
      let dispatchSpy, fetchStub, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch MATERIAL_FETCH_SUCCEEDED when material exists', async () => {
        let getStub = sinon.stub();
        let data = {
          "headers": [
            {
              "columns": [
                {
                  "key": "material_level_1",
                  "name": "Material Level 1",
                  "value": "Primary"
                },
                {
                  "key": "material_level_2",
                  "name": "Material Level 2",
                  "value": "Aluminium and Copper"
                }
              ],
              "key": "classification",
              "name": "Classification"
            }]
        };
        getStub.returns(Promise.resolve({data: data}));
        ConnectorRewired.__Rewire__('axios', {get: getStub});
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_SUCCEEDED',
          details: data
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({message: 'error has been occurred'}))});
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'error has been occurred'
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 404}}))
        });
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'Material : MT0001 is not found'
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when status code is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 500}}))
        });
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when status code is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 403}}))
        });
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });
    });

    describe('onDependencyDefinitionFetchRequest', () => {
      let dispatchSpy, fetchStub, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch DEPENDENCY_DEFINITION_FETCH_SUCCEEDED when dependency definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.resolve({data: 'dependency data'}))
        });
        await returnedObject.onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.be.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED',
          dependencyDefinitionId: 'materialClassifications',
          dependencyDefinition: 'dependency data'
        });
      });

      it('should dispatch DEPENDENCY_DEFINITION_FETCH_FAILED when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 404}}))
        });
        await returnedObject.onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.be.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId: 'materialClassifications',
          error: 'dependency definition: materialClassifications is not found'
        });
      });

      it('should dispatch DEPENDENCY_DEFINITION_FETCH_FAILED when status code is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 500}}))
        });
        await returnedObject.onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.be.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId: 'materialClassifications',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch DEPENDENCY_DEFINITION_FETCH_FAILED when status code is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 403}}))
        });
        await returnedObject.onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.be.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId: 'materialClassifications',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch DEPENDENCY_DEFINITION_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({message: 'error'}))});
        await returnedObject.onDependencyDefinitionFetch('materialClassifications');
        expect(dispatchSpy).to.be.calledWith({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId: 'materialClassifications',
          error: {message: 'error'}
        });
      });
    });

    describe('onMasterDataFetch', () => {
      it('should call dispatch with COMPONENT_MASTER_DATA_FETCH_SUCCEEDED when API returns data', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'masterData'}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('0304');
        expect(dispatchSpy).to.have.been.calledWith({
          type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED',
          masterData: 'masterData'
        });
      });

      it('should dispatch COMPONENT_MASTER_DATA_FETCH_FAILED when status code is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 404}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_MASTER_DATA_FETCH_FAILED',
          error: 'Master data is not found for MT0001'
        });
      });

      it('should dispatch COMPONENT_MASTER_DATA_FETCH_FAILED when status code is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 500}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_MASTER_DATA_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch COMPONENT_MASTER_DATA_FETCH_FAILED when status code is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 403}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'COMPONENT_MASTER_DATA_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should call dispatch with COMPONENT_MASTER_DATA_FETCH_FAILED when API returns error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject('error'))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onMasterDataFetch('0304');
        expect(dispatchSpy).to.have.been.calledWith({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: 'error'});
      });
    });

    describe('onUpdateMaterial', () => {
      let details;
      beforeEach(() => {
        details = {
          headers: [
            {
              columns: [
                {
                  name: 'Image',
                  key: 'image',
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
      it('should dispatch action UPDATE_MATERIAL_SUCCEEDED when API returns response', async () => {
        const dispatchSpy = sinon.spy();

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const postStub = sinon.stub();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details)
            .returns(Promise.resolve({data: {id: 'IRN0001'}})),
          post: postStub
            .withArgs([apiUrl('upload/static-files'), {}])
            .onFirstCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0006.png"}]}))
            .onSecondCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0007.png"}]}))
        });

        const pushSpy = sinon.spy();
        const successSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        ConnectorRewired.__Rewire__('toastr', {success: successSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateMaterial('IRN0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_SUCCEEDED', id: 'IRN0001'});
        expect(pushSpy).to.be.calledWith('/materials/IRN0001');
        expect(successSpy).to.be.calledWith('Success', 'The Material IRN0001 details are updated successfully');
      });

      it('should dispatch action UPDATE_MATERIAL_FAILED when API throws an error', async () => {
        const dispatchSpy = sinon.spy();

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const postStub = sinon.stub();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details).returns(Promise.reject({message: 'message'})),
          post: postStub
            .withArgs([apiUrl('upload/static-files'), {}])
            .onFirstCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0006.png"}]}))
            .onSecondCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0007.png"}]}))
        });

        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateMaterial('IRN0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_FAILED', error: {message: 'message'}});
        expect(pushSpy).to.not.be.calledWith('/materials/IRN0001');
      });

      it('should dispatch action UPDATE_MATERIAL_FAILED with response data when API throws an error and having response', async () => {
        const dispatchSpy = sinon.spy();

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const postStub = sinon.stub();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details).returns(Promise.reject({response: {data: 'message'}})),
          post: postStub
            .withArgs([apiUrl('upload/static-files'), {}])
            .onFirstCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0006.png"}]}))
            .onSecondCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0007.png"}]}))
        });

        const pushSpy = sinon.spy();

        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateMaterial('IRN0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_MATERIAL_FAILED', error: "message"});
        expect(pushSpy).to.not.be.calledWith('/materials/IRN0001');
      });
    });

    describe('onUpdateService', () => {
      let details;
      beforeEach(() => {
        details = {
          headers: [
            {
              columns: [
                {
                  name: 'Image',
                  key: 'image',
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
      it('should dispatch action UPDATE_SERVICE_SUCCEEDED when API returns response', async () => {
        window.fileList = [[{name: "MISI0006.png", type: "image/png"}], [{name: "MISI0007.png", type: "image/png"}]];
        const dispatchSpy = sinon.spy();

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('services'), details)
            .returns(Promise.resolve({data: {id: 'FDP0001'}}))
        });

        const pushSpy = sinon.spy();
        const successSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        ConnectorRewired.__Rewire__('toastr', {success: successSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateService('FDP0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_SUCCEEDED', id: 'FDP0001'});
        expect(pushSpy).to.be.calledWith('/services/FDP0001');
        expect(successSpy).to.be.calledWith('Success', 'The Service FDP0001 details are updated successfully');
      });

      it('should dispatch action UPDATE_SERVICE_SUCCEEDED when details contains array of static files', async () => {
        const expected = {
          general: {
            'image': [{id: "id", name: "MISI0006.png"}, {id: "id", name: "MISI0007.png"}, {
              name: 'MISI0002.png',
              id: "335355"
            }],
            'hsn Code': 1234
          }
        };

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const postStub = sinon.stub();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('services/FDP0001'), expected)
            .returns(Promise.resolve({data: {id: 'FDP0001'}})),
          post: postStub
            .withArgs(apiUrl('upload/static-files'), {})
            .onFirstCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0006.png"}]}))
            .onSecondCall().returns(Promise.resolve({data: [{id: "id", name: "MISI0007.png"}]}))
        });

        const pushSpy = sinon.spy();
        const successSpy = sinon.spy();
        const dispatchSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        ConnectorRewired.__Rewire__('toastr', {success: successSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateService('FDP0001', details);
        expect(appendSpy).to.be.calledWith('File', {name: "MISI0006.png", type: "image/png"});
        expect(appendSpy).to.be.calledWith('File', {name: "MISI0007.png", type: "image/png"});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_SUCCEEDED', id: 'FDP0001'});
        expect(pushSpy).to.be.calledWith('/services/FDP0001');
        expect(successSpy).to.be.calledWith('Success', 'The Service FDP0001 details are updated successfully');
      });

      it('should dispatch action UPDATE_SERVICE_FAILED when API throws an error', async () => {
        const dispatchSpy = sinon.spy();
        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('services'), details).returns(Promise.reject({message: 'message'})),
        });

        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateService('FDP0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_FAILED', error: {message: 'message'}});
        expect(pushSpy).to.not.be.calledWith('/services/FDP0001');
      });

      it('should dispatch action UPDATE_SERVICE_FAILED with response data when API throws an error and having response', async () => {
        const dispatchSpy = sinon.spy();

        const appendSpy = sinon.spy();
        window.FormData = sinon.stub().returns({append: appendSpy});
        const postStub = sinon.stub();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details).returns(Promise.reject({response: {data: 'message'}})),
        });

        const pushSpy = sinon.spy();

        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        await mapDispatchToProps(dispatchSpy).onUpdateService('FDP0001', details);
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_REQUESTED'});
        expect(dispatchSpy).to.have.been.calledWith({type: 'UPDATE_SERVICE_FAILED', error: "message"});
        expect(pushSpy).to.not.be.calledWith('/services/FDP0001');
      });
    });

    describe('onCancelUpdateMaterial', () => {
      it('should take the user to materials page', () => {
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        mapDispatchToProps(() => {
        }).onCancelUpdateMaterial("IRN0001");
        expect(pushSpy).to.be.calledWith('/materials/IRN0001');
      });
    });

    describe('onCancelUpdateService', () => {
      it('should take the user to services page', () => {
        const pushSpy = sinon.spy();
        ConnectorRewired.__Rewire__('browserHistory', {push: pushSpy});
        mapDispatchToProps(() => {
        }).onCancelUpdateService("FDP0001");
        expect(pushSpy).to.be.calledWith('/services/FDP0001');
      });
    });

    describe('onResetError', () => {
      it('should dispatch action RESET_UPDATE_ERROR', () => {
        const dispatchSpy = sinon.spy();
        mapDispatchToProps(dispatchSpy).onResetError();
        expect(dispatchSpy).to.have.been.calledWith({type: 'RESET_UPDATE_ERROR'});
      });
    });
  });
});
