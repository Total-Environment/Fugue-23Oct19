import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Package, __RewireAPI__ as PackageRewired} from "../../../src/components/package/index";

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('package', () => {
  describe('render', () => {
    it('should render error message when error exist', () => {
      let props = {error: 'Package data not found'};
      let wrapper = shallow(<Package {...props}/>);
      expect(wrapper).to.have.descendants('h3');
      expect(wrapper.find('h3')).to.have.text('Package data not found');
    });

    it('should render data', () => {
      const headers = [
        {
          columns: [
            {
              name: 'Material Code',
              key: 'material_code',
              value: '1234',
              dataType: {
                name: 'string',
                subType: null
              }
            },
            {
              name: 'Material Status',
              key: 'material_status',
              value: 'under development',
              dataType: {
                name: 'masterData',
                subType: null
              }
            }
          ],
          name: 'General',
          key: 'general'
        },
        {
          columns: [
            {
              name: 'Material Level 1',
              key: 'material_level 1',
              value: 'primary',
              dataType: {
                name: 'masterData',
                subType: null
              }
            }
          ],
          name: 'Classification',
          key: 'classification'
        }
      ];
      const classificationDefinition = {
          name: 'Classification Definition',
          key: 'classification_definition'
        };
      const props = {
        details: {
          isFetching: false,
          values: {
            componentComposition: {
              "componentCoefficients": [
                {
                  "componentType": "Material",
                  "unitOfMeasure": "kg",
                  "name": "Aluminium Channel",
                  "code": "ALM000004",
                  "coefficient": 3,
                  "wastagePercentages": [],
                  "totalWastagePercentage": 0,
                  "totalQuantity": 3
                },
                {
                  "componentType": "Material",
                  "unitOfMeasure": "kg",
                  "name": "Aluminium Channel",
                  "code": "ALM000005",
                  "coefficient": 4,
                  "wastagePercentages": [],
                  "totalWastagePercentage": 0,
                  "totalQuantity": 4
                }
              ]
            },
            headers: [
              ...headers,
              classificationDefinition
            ]
          }
        },
        cost: 'cost',
      };
      const wrapper = shallow(<Package {...props}/>);
      expect(wrapper.find('Component')).to.have.length(1);
      expect(wrapper.find('Component')).to.have.prop('details').deep.equal({headers});
      expect(wrapper.find('Component')).to.have.prop('composition', props.details.values.componentComposition);
      expect(wrapper.find('Component')).to.have.prop('classificationDefinition', classificationDefinition);
      expect(wrapper.find('Component')).to.have.prop('cost', 'cost');
      expect(wrapper.find('Component')).to.have.prop('componentLocation', 'Bangalore');
    });

    it('should render data if packageCostError exist with cost data', () => {
      const props = {
        details: {
          isFetching: false,
          values: {
            componentComposition: 'composition',
            headers: [
              {
                columns: [
                  {
                    name: 'Material Code',
                    key: 'material_code',
                    value: '1234',
                    dataType: {
                      name: 'string',
                      subType: null
                    }
                  },
                  {
                    name: 'Material Status',
                    key: 'material_status',
                    value: 'under development',
                    dataType: {
                      name: 'masterData',
                      subType: null
                    }
                  }
                ],
                name: 'General',
                key: 'general'
              },
              {
                columns: [
                  {
                    name: 'Material Level 1',
                    key: 'material_level 1',
                    value: 'primary',
                    dataType: {
                      name: 'masterData',
                      subType: null
                    }
                  }
                ],
                name: 'Classification',
                key: 'classification'
              }
            ]
          }
        },
        cost: 'cost',
        packageCostError: 'error has occurred',
      };
      const wrapper = shallow(<Package {...props}/>);
      expect(wrapper).to.have.descendants('Component');
    });

  });

  describe('componentDidMount', () => {
    it('should be able to call onPackageFetchRequest from componentDidMount', () => {
      let props = {
        details: null,
        onPackageFetchRequest: sinon.spy(),
        onPackageCostFetchRequest: sinon.spy(),
        packageCode: 'PACKAGE001'
      };

      let pkg = new Package(props);
      pkg.componentDidMount();
      expect(props.onPackageFetchRequest).to.have.been.calledWith('PACKAGE001');
    });

    it('should be able to call onPackageCostFetchRequest from componentDidMount', () => {
      let props = {
        details: null,
        onPackageFetchRequest: sinon.spy(),
        onPackageCostFetchRequest: sinon.spy(),
        packageCode: 'PACKAGE001'
      };

      let pkg = new Package(props);
      pkg.componentDidMount();
      expect(props.onPackageCostFetchRequest).to.have.been.called;
    });
  });

  describe('componentWillUnmount', () => {
    it('should be able to call onPackageDestroy when componentWillUnmount is called', () => {
      let props = {
        details: "details data",
        onPackageDestroy: sinon.spy(),
        packageCode: 'PACKAGE001'
      };

      var pkg = new Package(props);
      pkg.componentWillUnmount();
      expect(props.onPackageDestroy).to.have.been.called;
    });
  });

});
