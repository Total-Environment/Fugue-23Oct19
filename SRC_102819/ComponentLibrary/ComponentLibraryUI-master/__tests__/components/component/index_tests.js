import React from 'react';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import sinon from 'sinon';
import chaiEnzyme from 'chai-enzyme';
import {idFor} from '../../../src/helpers';
import {Component, __RewireAPI__ as ComponentRewired} from '../../../src/components/component'


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('component', () => {

  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        componentCode: 'MT0001',
        details: {
          headers: [
            {
              columns: [
                {
                  value:'Primary',
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  dataType: {
                    name: 'String',
                    subType: ''
                  }
                },
                {
                  value:'Primary',
                  name: 'Service Level 1',
                  key: 'service_level_1',
                  dataType: {
                    name: 'String',
                    subType: ''
                  }
                }
              ],
              name: 'Classification',
              key: 'classification'
            },
            {
              columns:[
                {
                  name: 'Component Code',
                  key: 'component_code',
                  dataType: {
                    name: 'String',
                    subType: ''
                  },
                  value: 'CLY001'
                },
                {
                  name: 'Component Name',
                  key: 'name',
                  dataType: {
                    name: 'String',
                    subType: ''
                  },
                  value: 'Name'
                },
                {
                  name: 'Can be Used as an Asset',
                  key: 'can_be_used_as_an_asset',
                  dataType: {
                    name: 'Boolean',
                    subType: ''
                  },
                  value: true
                },
                {
                  name: 'Image',
                  key: 'image',
                  value: [
                    {
                      url: "http://media1.santabanta.com/full6/Nature/Flowers/flowers-166a.jpg",
                      name: "MMBI0001.jpg",
                      id: "7fb8f298af07499c9de103f6cb72c6d3"
                    }
                  ],
                  dataType: {
                    name: "Array",
                    subType: {
                      name: "StaticFile",
                      subType: null
                    }
                  }
                }
              ],
              name: 'General',
              key: 'general'
            }
          ]
        },
        masterData: {},
        rates: [{
          'rate': {
            'controlBaseRate': {
              'value': 10.0,
              'currency': 'INR'
            },
            'landedRate': {
              'value': 35.700,
              'currency': 'INR'
            },
            'coefficients': [
              {
                'name': 'Custom Duties',
                'value': {
                  'value': 5.0,
                  'currency': 'INR'
                }
              },
              {
                'name': 'Location variance',
                'value': {
                  'value': 3.0,
                  'currency': 'INR'
                }
              },
              {
                'name': 'Market Flucuation',
                'factor': '0.04'
              },
              {
                'name': 'Freight',
                'value': {
                  'value': 2.0,
                  'currency': 'INR'
                }
              },
              {
                'name': 'Insurance Considered By Customs',
                'value': {
                  'value': 10.0,
                  'currency': 'INR'
                }
              },
              {
                'name': 'Clearning Charges By Customs',
                'value': {
                  'value': 5.0,
                  'currency': 'INR'
                }
              },
              {
                'name': 'Tax Variance',
                'factor': '0.03'
              }
            ]
          },
          'location': 'Hyderabad',
          'typeOfPurchase': 'DOMESTIC INTER-STATE',
          'id': 'IRN000002',
          'appliedOn': '2017-01-30T00:00:00Z',
        }],
        rateHistoryLink: '/materials/MT0001/rate-history',
        componentType: "material",
      };
      wrapper = shallow(<Component {...props}/>);
    });

    it('should render sfg composition view if component type is sfg',() => {
      props.componentType = 'sfg';
      props.details = {
        "code": "FLR000006",
        "group": "FLOORING | DADO | PAVIOUR",
        "headers": [
          {
            "columns": [
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079ac40d552a14321a2d"
                },
                "key": "sfg_level_1",
                "name": "SFG Level 1",
                "value": "FLOORING | DADO | PAVIOUR"
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079ac40d552a14321a2e"
                },
                "key": "sfg_level_2",
                "name": "SFG Level 2",
                "value": "Flooring"
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079ac40d552a14321a2f"
                },
                "key": "sfg_level_3",
                "name": "SFG Level 3",
                "value": "Natural Stone"
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079ac40d552a14321a30"
                },
                "key": "sfg_level_4",
                "name": "SFG Level 4",
                "value": "Italian Marble"
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079ac40d552a14321a31"
                },
                "key": "sfg_level_5",
                "name": "SFG Level 5",
                "value": null
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079bc40d552a14321a32"
                },
                "key": "sfg_level_6",
                "name": "SFG Level 6",
                "value": null
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "5928079bc40d552a14321a33"
                },
                "key": "sfg_level_7",
                "name": "SFG Level 7",
                "value": null
              }
            ],
            "key": "classification",
            "name": "Classification"
          },
          {
            "columns": [
              {
                "dataType": {
                  "name": "Autogenerated",
                  "subType": "SFG Code"
                },
                "key": "sfg_code",
                "name": "SFG Code",
                "value": "FLR000006"
              },
              {
                "dataType": {
                  "name": "String",
                  "subType": null
                },
                "key": "wbs_code",
                "name": "WBS Code",
                "value": "12"
              },
              {
                "dataType": {
                  "name": "String",
                  "subType": null
                },
                "key": "short_description",
                "name": "Short Description",
                "value": "23"
              },
              {
                "dataType": {
                  "name": "String",
                  "subType": null
                },
                "key": "unit_of_measure",
                "name": "Unit Of Measure",
                "value": "23"
              },
              {
                "dataType": {
                  "name": "String",
                  "subType": null
                },
                "key": "hsn_code",
                "name": "HSN Code",
                "value": null
              },
              {
                "dataType": {
                  "name": "MasterData",
                  "subType": "590c5a94c40d554e2c3c53fb"
                },
                "key": "sfg_status",
                "name": "SFG Status",
                "value": "Inactive"
              }
            ],
            "key": "general",
            "name": "General"
          },
          {
            "columns": [
              {
                "dataType": {
                  "name": "StaticFile",
                  "subType": null
                },
                "key": "method_of_measurement",
                "name": "Method of Measurement",
                "value": null
              },
              {
                "dataType": {
                  "name": "CheckList",
                  "subType": null
                },
                "key": "general_wo_terms",
                "name": "General WO Terms",
                "value": null
              },
              {
                "dataType": {
                  "name": "CheckList",
                  "subType": null
                },
                "key": "special_wo_terms",
                "name": "Special WO Terms",
                "value": null
              },
              {
                "dataType": {
                  "name": "Money",
                  "subType": null
                },
                "key": "last_purchase_rate",
                "name": "Last Purchase Rate",
                "value": null
              },
              {
                "dataType": {
                  "name": "Money",
                  "subType": null
                },
                "key": "weighted_average_purchase_rate",
                "name": "Weighted Average Purchase Rate",
                "value": {
                  "amount": 23,
                  "currency": "Pounds"
                }
              },
              {
                "dataType": {
                  "name": "Unit",
                  "subType": "%"
                },
                "key": "purchase_rate_threshold_%",
                "name": "Purchase Rate Threshold %",
                "value": null
              },
              {
                "dataType": {
                  "name": "Array",
                  "subType": {
                    "name": "String",
                    "subType": null
                  }
                },
                "key": "approved_vendors",
                "name": "Approved Vendors",
                "value": null
              }
            ],
            "key": "purchase",
            "name": "Purchase"
          },
          {
            "columns": [
              {
                "dataType": {
                  "name": "Int",
                  "subType": null
                },
                "key": "wo_lead_time_in_days",
                "name": "WO Lead Time in Days",
                "value": null
              },
              {
                "dataType": {
                  "name": "Int",
                  "subType": null
                },
                "key": "vendor_mobilisation_time_in_days",
                "name": "Vendor Mobilisation Time in Days",
                "value": null
              },
              {
                "dataType": {
                  "name": "Int",
                  "subType": null
                },
                "key": "minimum_order_quantity",
                "name": "Minimum Order Quantity",
                "value": null
              }
            ],
            "key": "planning",
            "name": "Planning"
          },
          {
            "columns": [
              {
                "dataType": {
                  "name": "Array",
                  "subType": {
                    "name": "String",
                    "subType": null
                  }
                },
                "key": "governing_standard",
                "name": "Governing Standard",
                "value": null
              },
              {
                "dataType": {
                  "name": "CheckList",
                  "subType": null
                },
                "key": "quality_checklist",
                "name": "Quality Checklist",
                "value": null
              },
              {
                "dataType": {
                  "name": "StaticFile",
                  "subType": null
                },
                "key": "method_statement",
                "name": "Method Statement",
                "value": null
              },
              {
                "dataType": {
                  "name": "StaticFile",
                  "subType": null
                },
                "key": "safety_requirements",
                "name": "Safety Requirements",
                "value": null
              }
            ],
            "key": "quality",
            "name": "Quality"
          },
          {
            "columns": [
              {
                "dataType": {
                  "name": "Autogenerated",
                  "subType": "Date Created"
                },
                "key": "date_created",
                "name": "Date Created",
                "value": "2017-06-19T06:38:41.567Z"
              },
              {
                "dataType": {
                  "name": "Autogenerated",
                  "subType": "Created By"
                },
                "key": "created_by",
                "name": "Created By",
                "value": "TE"
              },
              {
                "dataType": {
                  "name": "Autogenerated",
                  "subType": "Date Last Amended"
                },
                "key": "date_last_amended",
                "name": "Date Last Amended",
                "value": "2017-06-19T06:38:41.567Z"
              },
              {
                "dataType": {
                  "name": "Autogenerated",
                  "subType": "Last Amended By"
                },
                "key": "last_amended_by",
                "name": "Last Amended By",
                "value": "TE"
              }
            ],
            "key": "system_logs",
            "name": "System Logs"
          }
        ],
      };
      props.composition = {
        "componentCoefficients": [
          {
            "componentType": "Service",
            "unitOfMeasure": "m2",
            "name": "Short Description",
            "code": "FDP1027",
            "coefficient": 10,
            "wastagePercentages": [
              {
                "name": "test3",
                "value": 2
              },
              {
                "name": "test4",
                "value": 3
              }
            ],
            "totalWastagePercentage": 5,
            "totalQuantity": 10.5
          },
          {
            "componentType": "Material",
            "unitOfMeasure": "Nos",
            "name": "Aluminium Straight Edge",
            "code": "ALM000039",
            "coefficient": 10,
            "wastagePercentages": [
              {
                "name": "test3",
                "value": 110
              },
              {
                "name": "test4",
                "value": 20
              }
            ],
            "totalWastagePercentage": 130,
            "totalQuantity": 23
          }
        ]
      };
      props.locations = ['Benguluru','Hyderabad'];
      props.componentCostError = "error";
      props.cost=null;
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.descendants('ComponentCompositionView');
      expect(wrapper.find('ComponentCompositionView')).to.have.prop('composition').eql(props.composition);
      expect(wrapper.find('ComponentCompositionView')).to.have.prop('cost');
      expect(wrapper.find('ComponentCompositionView')).to.have.prop('error').eql('error');
    });

    it('should return a div with proper class when details and rates exist', () => {
      ComponentRewired.__Rewire__('styles', {component: 'component'});
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper.prop('className')).to.equal('component');
    });

    it('should render details when details are present', () => {
      ComponentRewired.__Rewire__('styles', {component: 'component'});
      props.componentType = 'material';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(1).descendants('Collapse');
      expect(wrapper).to.have.exactly(4).descendants('CollapsePanel');
      expect(wrapper.find('Collapse')).to.have.prop('defaultActiveKey').to.deep.equal(['Classification', 'General', 'rates','rentalRates']);
      expect(wrapper).to.have.exactly(1).descendants('Rate');
    });

    it('should pass ComponentCode and ComponentType as context types', () => {
      wrapper = shallow(<Component {...props}/>);
      const context = wrapper.instance().getChildContext();
      expect(context).to.deep.equal({componentCode: 'MT0001', componentType: 'material'});
    });

    it('should not render Id and group', () => {
      props.details.id = 'MT0001';
      props.details.group = 'Safety';
      props.componentType = 'material';
      ComponentRewired.__Rewire__('styles', {component: 'component'});
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(4).descendants('CollapsePanel');
    });

    it('should pass MasterData list as SubType for masterData if masterData for that subType is present only if editable is true', () => {
      props = {
        details: {
          headers: [
            {
              columns: [
                {
                  value:'Primary',
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  dataType: {
                    name: 'MasterData',
                    subType: ''
                  }
                }
              ],
              name: 'Classification',
              key: 'classification'
            },
          ]
        },
        masterData: {
          123: {values: ['123', '456'], status: 'fetched'}
        },
        editable: true,
        componentType: "material"
      };
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.descendants('DataType');
      const expected = {
        value: 'Primary',
        dataType: {
          name: 'MasterData',
          subType: '123',
          values: {values: ['123', '456'], status: 'fetched'}
        }
      };
      // expect(wrapper.find('DataType')).to.have.prop('columnValue').deep.equal(expected);
    });

    it('should call onColumnChange when DataType calls onChange', () => {
      props = {
        details: {
          headers: [
            {
              columns: [
                {
                  value:'Primary',
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  dataType: {
                    name: 'String',
                    subType: ''
                  }
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        },
        onColumnChange: sinon.spy(),
        componentType: "material"
      };
      wrapper = shallow(<Component {...props}/>);
      wrapper.find('DataType').simulate('change', 'Secondary');
      expect(props.onColumnChange).to.have.been.calledWith('classification', 'material_level_2', 'Secondary','Material Level 2');
    });

    it('should not render image when editable is false', () => {
      expect(wrapper).to.not.contain(<span>image</span>);
    });

    it('should render image when editable is true', () => {
      props.editable = true;
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.contain(<span>Image</span>);
    });

    it('should render a link to view history screen if props have rates and material code', () => {
      const node = wrapper.find('CollapsePanel').at(2).prop('header');
      const rate = shallow(node);
      expect(rate).to.have.descendants('Link');
      expect(rate.find('Link')).to.have.prop('to', '/materials/MT0001/rate-history');
    });

    it('should render a link to view history screen even if props does not have rates and material code', () => {
      props.rates = [];
      wrapper = shallow(<Component {...props}/>);
      const node = wrapper.find('CollapsePanel').last().prop('header');
      const rate = shallow(node);
      expect(rate).to.have.descendants('Link');
    });

    it('should not render rental rate when componentType is service', ()=>{
      props.componentType = 'service';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(3).descendants('CollapsePanel');
    });

    it('should not have rental rate in defaultActiveKey when componentType is service',()=>{
      props.componentType = 'service';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper.find('Collapse')).to.have.prop('defaultActiveKey').to.deep.equal(['Classification', 'General', 'rates'])
    });

    it('should not render rental rate if editable is true',()=>{
      props.componentType = 'material';
      props.editable = true;
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(2).descendants('CollapsePanel');
    });

    it('should not render rental rate if material is not an asset',()=>{
      props.details = {
        headers: [
          {
            columns: [
              {
                value:'Primary',
                name: 'Material Level 2',
                key: 'material_level_2',
                dataType: {
                  name: 'String',
                  subType: ''
                }
              },
              {
                value:'Primary',
                name: 'Service Level 1',
                key: 'service_level_1',
                dataType: {
                  name: 'String',
                  subType: ''
                }
              }
            ],
            name: 'Classification',
            key: 'classification'
          },
          {
            columns:[
              {
                name: 'Component Code',
                key: 'component_code',
                dataType: {
                  name: 'String',
                  subType: ''
                },
                value: 'CLY001'
              },
              {
                name: 'Component Name',
                key: 'name',
                dataType: {
                  name: 'String',
                  subType: ''
                },
                value: 'Name'
              },
              {
                name: 'Can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                dataType: {
                  name: 'Boolean',
                  subType: ''
                },
                value: false
              },
              {
                name: 'Image',
                key: 'image',
                value: [
                  {
                    url: "http://media1.santabanta.com/full6/Nature/Flowers/flowers-166a.jpg",
                    name: "MMBI0001.jpg",
                    id: "7fb8f298af07499c9de103f6cb72c6d3"
                  }
                ],
                dataType: {
                  name: "Array",
                  subType: {
                    name: "StaticFile",
                    subType: null
                  }
                }
              }
            ],
            name: 'General',
            key: 'general'
          }
        ]
      };
      props.editable = false;
      props.componentType = 'material';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(3).descendants('CollapsePanel');
    });

    it('should not have rental rate in defaultActiveKey when Can be Used as an Asset is false',()=>{
      props.details = {
        headers: [
          {
            columns: [
              {
                value:'Primary',
                name: 'Material Level 2',
                key: 'material_level_2',
                dataType: {
                  name: 'String',
                  subType: ''
                }
              },
              {
                value:'Primary',
                name: 'Service Level 1',
                key: 'service_level_1',
                dataType: {
                  name: 'String',
                  subType: ''
                }
              }
            ],
            name: 'Classification',
            key: 'classification'
          },
          {
            columns:[
              {
                name: 'Component Code',
                key: 'component_code',
                dataType: {
                  name: 'String',
                  subType: ''
                },
                value: 'CLY001'
              },
              {
                name: 'Component Name',
                key: 'name',
                dataType: {
                  name: 'String',
                  subType: ''
                },
                value: 'Name'
              },
              {
                name: 'Can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                dataType: {
                  name: 'Boolean',
                  subType: ''
                },
                value: false
              },
              {
                name: 'Image',
                key: 'image',
                value: [
                  {
                    url: "http://media1.santabanta.com/full6/Nature/Flowers/flowers-166a.jpg",
                    name: "MMBI0001.jpg",
                    id: "7fb8f298af07499c9de103f6cb72c6d3"
                  }
                ],
                dataType: {
                  name: "Array",
                  subType: {
                    name: "StaticFile",
                    subType: null
                  }
                }
              }
            ],
            name: 'General',
            key: 'general'
          }
        ]
      };
      props.editable = false;
      props.componentType = 'material';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper.find('Collapse')).to.have.prop('defaultActiveKey').to.deep.equal(['Classification', 'General', 'rates'])
    });

    it('should not show rate error message even if there is rateError',()=>{
      props.rateserror="some error";
      props.componentType='material';
      props.rates=null;
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper).to.have.exactly(4).descendants('CollapsePanel');
      expect(wrapper.find('CollapsePanel').at(2).children()).to.not.have.descendants('Rates');
      expect(wrapper.find('CollapsePanel').at(2).children()).to.have.id('rateserror');
    });
    it('should show error msg when error msg exist',()=>{
      props.rateserror="some error";
      props.componentType='material';
      wrapper = shallow(<Component {...props}/>);
      expect(wrapper.find(`#${idFor('rateserror')}`)).to.include.text('some error');
    });
    it('should not show rate section during create', ()=>{
       props.componentType = 'material';
       props.editable = true;
       props.rates = null;
       wrapper = shallow(<Component {...props}/>);
       expect(wrapper).to.have.exactly(2).descendants('CollapsePanel');
    });
      it('should not show rate section during create', ()=>{
          props.componentType = 'service';
          props.editable = true;
          props.rates = null;
          wrapper = shallow(<Component {...props}/>);
          expect(wrapper).to.have.exactly(2).descendants('CollapsePanel');
      });
  });

  describe('componentDidMount and componentWillReceiveProps', () => {
    let props;
    beforeEach(() => {
      props = {
        details: {
          headers: [
            {
              columns: [
                {
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  value: 'Primary',
                  dataType: {
                    name: 'MasterData',
                    subType: '123'
                  },
                  editable: true
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        },
        masterData: {},
        onMasterDataFetch: sinon.spy(),
        editable: true
      };
    });
    it('componentDidMount should fetch MasterData if masterData is not present and editable', () => {
      new Component(props).componentDidMount();
      expect(props.onMasterDataFetch).to.have.been.calledWith('123');
    });
    it('componentWillReceiveProps fetch MasterData if masterData is not present', () => {
      new Component(props).componentWillReceiveProps(props);
      expect(props.onMasterDataFetch).to.have.been.calledWith('123');
    });

    it('componentWillReceiveProps should not fetch MasterData for id and group', () => {
      props = {
        details: {
          classification: {
            'Material Level 1': {
              value: 'Primary',
              dataType: {
                name: 'MasterData',
                subType: '123'
              },
              editable: true
            },
          },
          id: 'MT0001'
        },
        masterData: {},
        onMasterDataFetch: sinon.spy(),
        editable: true
      };
      //  new Component(props).
    });
  });
});
