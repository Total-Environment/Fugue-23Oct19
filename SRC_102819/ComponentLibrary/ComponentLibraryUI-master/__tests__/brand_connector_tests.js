import sinon from 'sinon';
import { apiUrl } from '../src/helpers';
import { mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired } from '../src/brand_connector';
import chai, { expect } from 'chai';

import sinonChai from 'sinon-chai';


chai.use(sinonChai);

describe('BrandConnector', () => {
  describe('mapStateToProps', () => {
    it('Should transform state and props', () => {
      const props = { params: { materialCode: 'MT0001', brandCode: 'BSY0001' } };
      const state = {
        reducer: {
          components: { 'MT0001': { name: "abc" } },
          fetchBrandError: '',
          newBrandError: '',
          brandDefinition: {},
          brandDefinitionError: '',
          brandAdding: false,
          newBrandError: '',
          masterData: {},
          addedBrand: false
        }
      };

      const actual = mapStateToProps(state, props);
      const expected = {
        brandCode: 'BSY0001',
        materialCode: 'MT0001',
        details: { name: "abc" },
        detailsError: '',
        brandDefinition: {},
        brandDefinitionError: '',
        brandAdding: false,
        newBrandError: '',
        addedBrand: false,
        isSavingBrand: false,
        action: undefined,
        updatedBrand: false,
        masterData: {}
      };
      expect(actual).to.deep.equal(expected)
    })
  });

  describe('mapDispatchToProps', () => {
    describe('onBrandFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch BRAND_FETCH_SUCCEEDED when material exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'materialDetails' })) });
        await returnedObject.onBrandFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_FETCH_SUCCEEDED',
          details: 'materialDetails'
        });
      });

      it('should dispatch BRAND_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ message: 'Brand not found.' })) });

        await returnedObject.onBrandFetchRequest('MT0001');

        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_FETCH_FAILED',
          materialCode: 'MT0001',
          detailserror: 'Brand not found.'
        });
      });
      it('should dispatch BRAND_FETCH_FAILED when response status is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 500 } }))
        });
        await returnedObject.onBrandFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_FETCH_FAILED',
          materialCode: 'SFT00003',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch BRAND_FETCH_FAILED when response status is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 404 } }))
        });
        await returnedObject.onBrandFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_FETCH_FAILED',
          materialCode: 'SFT00003',
          detailserror: 'No Brand with Material code: SFT00003 is found.'
        });
      });

      it('should dispatch BRAND_FETCH_FAILED when response status is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 403 } }))
        });
        await returnedObject.onBrandFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_FETCH_FAILED',
          materialCode: 'SFT00003',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });
    });

    describe('onBrandDefinitionFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch BRAND_DEFINITION_FETCH_SUCCEEDED when brand definition exists', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.resolve({ data: 'brandDefinition' })) });
        await returnedObject.onBrandDefinitionFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_DEFINITION_FETCH_SUCCEEDED',
          brandDefinition: 'brandDefinition'
        });
      });

      it('should dispatch BRAND_DEFINITION_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', { get: sinon.stub().returns(Promise.reject({ message: 'Brand Definition not found.' })) });

        await returnedObject.onBrandDefinitionFetchRequest();

        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'Brand Definition not found.'
        });
      });

      it('should dispatch BRAND_DEFINITION_FETCH_FAILED when response status is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 500 } }))
        });
        await returnedObject.onBrandDefinitionFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch BRAND_DEFINITION_FETCH_FAILED when response status is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 404 } }))
        });
        await returnedObject.onBrandDefinitionFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'No Brand Definition is found.'
        });
      });

      it('should dispatch BRAND_DEFINITION_FETCH_FAILED when response status is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({ response: { status: 403 } }))
        });
        await returnedObject.onBrandDefinitionFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'BRAND_DEFINITION_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });
    });

    describe('onAddBrand', () => {
      let details;
      beforeEach(() => {
        details = {
          "headers": [
            {
              "key": "purchase",
              "name": "Purchase",
              "columns": [
                {
                  "key": "approved_brands",
                  "name": "Associated Brands",
                  "value": [
                    {
                      "columns": [
                        {
                          "key": "brand_code",
                          "name": "Brand Code",
                          "value": null
                        },
                        {
                          "key": "manufacturer's_name",
                          "name": "Manufacturer's Name",
                          "value": "Jaguar"
                        },
                        {
                          "key": "brand/series",
                          "name": "Brand/Series",
                          "value": null
                        },
                        {
                          "key": "manufacturer's_code",
                          "name": "Manufacturer's Code",
                          "value": null
                        },
                        {
                          "key": "manufacturer's_specification",
                          "name": "Manufacturer's Specification",
                          "value": null
                        },
                        {
                          "key": "material_safety_data_sheet",
                          "name": "Material Safety Data Sheet",
                          "value": null
                        },
                        {
                          "key": "technical_drawing",
                          "name": "Technical Drawing",
                          "value": null
                        },
                        {
                          "key": "3d_model",
                          "name": "3D Model",
                          "value": null
                        },
                        {
                          "key": "country_of_manufacture",
                          "name": "Country of Manufacture",
                          "value": null
                        },
                        {
                          "key": "warranty_period_in_years",
                          "name": "Warranty Period in Years",
                          "value": null
                        },
                        {
                          "key": "status",
                          "name": "Status",
                          "value": "Approved"
                        },
                        {
                          "key": "approved_vendors",
                          "name": "Approved Vendors",
                          "value": null
                        },
                        {
                          "key": "shelf_life_in_days",
                          "name": "Shelf Life in Days",
                          "value": null
                        },
                        {
                          "key": "packing_list",
                          "name": "Packing List",
                          "value": null
                        }
                      ]
                    }
                  ]
                }
              ]
            }],
          "group": "Aluminium and Copper",
          "id": "ALM000005"
        };
      });

      it('should dispatch action ADD_BRAND_SUCCEEDED when API returns response', async () => {
        const dispatchSpy = sinon.spy();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details)
            .returns(Promise.resolve({ data: details }))
        });

        const successSpy = sinon.spy();
        ConnectorRewired.__Rewire__('toastr', { success: successSpy });
        await mapDispatchToProps(dispatchSpy).onAddOrEditBrand('ALM000005', details);
        expect(dispatchSpy).to.have.been.calledWith({ type: 'ADD_BRAND_REQUESTED' });
        expect(dispatchSpy).to.have.been.calledWith({ type: 'ADD_BRAND_SUCCEEDED', details: details });
        expect(successSpy).to.be.calledWith('success', 'Brand details created successfully');
      });

      it('should dispatch action ADD_BRAND_FAILED when API returns response', async () => {
        const dispatchSpy = sinon.spy();
        const putStub = sinon.stub();

        ConnectorRewired.__Rewire__('axios', {
          put: putStub
            .withArgs(apiUrl('materials'), details)
            .returns(Promise.reject({ message: 'message' }))
        });

        await mapDispatchToProps(dispatchSpy).onAddOrEditBrand('ALM000005', details);
        expect(dispatchSpy).to.have.been.calledWith({ type: 'ADD_BRAND_REQUESTED' });
        expect(dispatchSpy).to.have.been.calledWith({ type: 'ADD_BRAND_FAILED', error: { message: 'message' } });
      });
    })
  })
});
