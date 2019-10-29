import React from 'react';
import {shallow} from 'enzyme';
import {EditComponent} from '../../../src/components/edit-component';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('EditComponent', () => {
  let props, onMaterialFetchRequestSpy,
    onDependencyDefinitionFetchSpy,
    onUpdateMaterialSpy,
    onCancelUpdateMaterialSpy,
    onCancelUpdateServiceSpy,
    onResetErrorSpy,
    onUpdateServiceSpy;
  beforeEach(() => {
    onMaterialFetchRequestSpy = sinon.spy();
    onDependencyDefinitionFetchSpy = sinon.spy();
    onUpdateMaterialSpy = sinon.spy();
    onCancelUpdateMaterialSpy = sinon.spy();
    onCancelUpdateServiceSpy = sinon.spy();
    onResetErrorSpy = sinon.spy();
    onUpdateServiceSpy = sinon.spy();
    props = {
      onMaterialFetchRequest: onMaterialFetchRequestSpy,
      onDependencyDefinitionFetch: onDependencyDefinitionFetchSpy,
      componentType: 'material',
      dependencyDefinitions: {},
      componentCode: 'IRN00001',
      onUpdateMaterial: onUpdateMaterialSpy,
      componentUpdateError: '',
      onCancelUpdateMaterial:  onCancelUpdateMaterialSpy,
      onCancelUpdateService: onCancelUpdateServiceSpy,
      onResetError: onResetErrorSpy,
      onUpdateService: onUpdateServiceSpy,
      masterData:{}
    };
  });

  describe('componentDidMount', () => {
    it('should call onDependencyDefinitionFetch when dependencyDefinition is not present', () => {
      props.details = {materialName: 'name'};
      const wrapper = shallow(<EditComponent {...props}/>);
      wrapper.instance().componentDidMount();
      expect(onDependencyDefinitionFetchSpy).to.have.been.calledWith('materialClassifications');
    });

    it('should call onMaterialDefinitionFetch when details are not present and componentType is material', () => {
      props.dependencyDefinitions = {'IRN000001': {}};
      const wrapper = shallow(<EditComponent {...props}/>);
      wrapper.instance().componentDidMount();
      expect(onMaterialFetchRequestSpy).to.have.been.calledWith('IRN00001');
    });
  });

  describe('componentWillReceiveProps', () => {
    it('should update details', () => {
      props.details = undefined;
      props.dependencyDefinitions = {'materialClassifications': {name: 'definition'}};
      const wrapper = shallow(<EditComponent {...props}/>);

      const nextProps = {
        details: {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        }
      };
      const expected = {
        details: {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''}
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        }
      };
      wrapper.setProps(nextProps);
      expect(wrapper.find('Component')).to.have.prop('details').deep.equal(expected.details);
    });
  });

  describe('render', () => {
    it('should render loading when details and dependency definition are not present', () => {
      const wrapper = shallow(<EditComponent {...props}/>);
      expect(wrapper).to.have.descendants('Loading');
    });

    context('when details and dependency definition present', () => {
      let wrapper;
      beforeEach(() => {
        props.dependencyDefinitions = {
          materialClassifications: {
            isFetching: false,
              values: {
              columnList: ["Material Level 1", "Material Level 2"],
                block: {
                name: 'Dependency',
                  children: [{
                  name: 'Primary',
                  children: [{
                    name: 'Clay Material',
                    children: []
                  }]
                },
                {name: 'secondary',
                  children: [{
                  name:'Synthetic Flooring',
                    children: []
                  }]
                }]
              }
            },
          },
          serviceClassifications: {
            isFetching: false,
            values: {
              columnList: ["Service Level 1", "Service Level 2"],
              block: {
                name: 'Dependency',
                children: [{
                  name: 'Civil Works',
                  children: [{
                    name: 'Earthwork',
                    children: []
                  }]
                },
                  {name: 'FLOORING | DADO | PAVIOUR',
                    children: [{
                      name:'Flooring',
                      children: []
                    }]
                  }]
              }
            },
          }
        };
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        wrapper = shallow(<EditComponent {...props}/>);
      });

      it('should render component when details and dependency definition exist', () => {
        const expected = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  name: 'Material Level 1',
                  key: 'material_level_1'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        expect(wrapper).to.have.descendants('Component');
        expect(wrapper.find('Component')).to.have.prop('details').deep.equal(expected);
      });

      it('should change the columns in details when column value is changed', () => {
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Code',
                  key: 'material_code',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'IRN0001'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        const expected = {details: {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Primary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Code',
                  key: 'material_code',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'IRN0001'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        }};
        const wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component')
          .simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        expect(wrapper.find('Component')).to.have.prop('details').deep.equal(expected.details);
      });

      it('should have column values as empty whose level of dependency is greater than changed column', () => {
        props.dependencyDefinitions = {
          materialClassifications: {
            isFetching: false,
            values: {
              columnList: ["Material Level 1", "Material Level 2","Material Level 3", "Material Level 4"],
              block: {
                name: 'Dependency',
                children: [{
                  name: 'Primary',
                  children: [{
                    name: 'Clay Material',
                    children: [{
                      name: 'soil',
                      children: [{
                        name: 'Filler',
                        children:[]
                      }]
                    }]
                  }]
                },
                  {name: 'secondary',
                    children: [{
                      name:'Synthetic Flooring',
                      children: [{
                        name: 'Laminate Flooring',
                        children: [{
                          name: 'Flooring',
                          children:[]
                        }]
                      },
                      {
                        name: 'Skirting',
                        children:[{
                          name: null,
                          children:[]
                        }]
                      }]
                    }]
                  }]
              }
            },
          }
        };
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Synthetic Flooring'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 3',
                  key: 'material_level_3',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Laminate Flooring'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 4',
                  key: 'material_level_4',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Flooring'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ],
        };
        const wrapper = shallow(<EditComponent {...props}/>);
        const expected = {details: {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Synthetic Flooring'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 3',
                  key: 'material_level_3',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Skirting'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 4',
                  key: 'material_level_4',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: ''
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        }};
        wrapper.find('Component')
          .simulate('columnChange', 'classification', 'material_level_3', 'Skirting','Material Level 3');
        expect(wrapper.find('Component')).to.have.prop('details').deep.equal(expected.details);
      });

      it('should render Material code as title', () => {
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                }
              ],
              name: 'Classification',
              key: 'classification',
              value: 'Primary'
            },
            {
              columns: [
                {
                  name: 'Material Code',
                  key: 'material_code',
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  value: 'IRN0001'
                }
              ],
              name: 'General',
              key: 'general'
            }
          ]
        };
        const wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.include.text('IRN0001');
      });

      it('should call onMasterDataFetch,', () => {
        const onMasterDataFetchSpy = sinon.spy();
        props.onMasterDataFetch = onMasterDataFetchSpy;
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('masterDataFetch');
        expect(onMasterDataFetchSpy).to.be.called;
      });

      it('should render form', () => {
        expect(wrapper).to.have.descendants('form');
      });

      it('should call onUpdateMaterial when form is submitted', () => {
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'wood'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        props.componentType = 'material';
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_2', 'Clay Material','Material Level 2');
        wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
        const expected = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Primary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Clay Material'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };

        expect(props.onUpdateMaterial).to.have.been.calledWith('IRN00001',expected);
        expect(preventDefaultSpy).to.have.been.called;
      });

      it('should not call onUpdateMaterial and show an alert when dependency definition does not match', () => {
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'wood'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 3',
                  key: 'material_level_3',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'other'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        props.componentType = 'material';
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_3', 'Flooring','Material Level 3');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onUpdateMaterial).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should disable update when updating is true', () => {
        props.componentUpdating = true;
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper.find('input[type="submit"]')).to.have.prop('disabled');
      });

      it('should not call onUpdateMaterial and show an alert when required column has null value', () => {
        props.details = {
          headers: [
            {
              columns: [
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 1',
                  key: 'material_level_1',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'secondary'
                },
                {
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'wood'
                }
              ],
              name: 'Classification',
              key: 'classification'
            },
            {
              columns: [
                {
                  name: 'Unit of Measure',
                  key: 'unit_of_measure',
                  value: 'MT',
                  isRequired: true,
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  }
                }
              ],
              key: 'general',
              name: 'General'
            }
          ]
        };
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'secondary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_2', 'Synthetic Flooring','Material Level 2');
        wrapper.find('Component').simulate('columnChange', 'general', 'unit_of_measure', '');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onUpdateMaterial).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should render ConfirmationDialog when cancel button is clicked', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('message').to.equal('The changes made to Material IRN00001 will not be saved. Do you wish to continue?');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onCancel when cancel button is clicked and yes button is clicked on ConfirmationDialog', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('yes');
        expect(props.onCancelUpdateMaterial).to.have.been.calledWith('IRN00001');
      });

      it('should not call onCancel when cancel button is clicked and no button is clicked on ConfirmationDialog', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('no');
        expect(props.onCancelUpdateMaterial).to.be.not.called;
      });

      it('should render AlertDialog with message when componentCreateError is passed', () => {
        props.componentUpdateError = {message:'Error'};
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.have.descendants('AlertDialog');
       expect(wrapper.find('AlertDialog')).to.have.prop('message').to.equal('Error');
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onResetError when componentCreateError is passed and click ok on alertDialog', () => {
        props.componentUpdateError = {message:'Error'};
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.equal('Error');
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
        wrapper.find('AlertDialog').simulate('close');
        expect(props.onResetError).to.be.called;
      });

      it('should show warning message when unit of measure is changed', () => {
        props.details = {
          headers: [
            {
              name: 'General',
              key: 'general',
              columns: [
                {
                  name: 'material_name',
                  key: 'Material Name',
                  dataType: {
                    name: 'String',
                    subType: null
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Iron and Steel'
                },
                {
                  name: 'Unit of Measure',
                  key: 'unit_of_measure',
                  dataType: {
                    name: 'String',
                    subType: null
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: "MT",
                  isRequired: true,
                },
                {
                  name: 'Material Code',
                  key: 'material_code',
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'IRN0001'
                }
              ]
            }
          ]
        };
        const message = 'Changing Unit of Measure will impact existing purchase orders in Fugue. Existing purchase orders need to be closed before the Unit of Measure is updated.';
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'general', 'unit_of_measure', 'MTT','Unit of Measure');
        expect(wrapper).to.have.descendants('AlertDialog');
        wrapper.find('AlertDialog').simulate('ok');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.deep.equal(message);
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;

      });

      it('should show warning message when material status is changed', () => {
        props.details = {
          headers: [
            {
              name: 'General',
              key: 'general',
              columns: [
                {
                  name: 'material_name',
                  key: 'Material Name',
                  dataType: {
                    name: 'String',
                    subType: null
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Iron and Steel'
                },
                {
                  name: 'Unit of Measure',
                  key: 'unit_of_measure',
                  dataType: {
                    name: 'String',
                    subType: null
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: "MT",
                  isRequired: true,
                },
                {
                  name: 'Material Code',
                  key: 'material_code',
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId'
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'IRN0001'
                },
                {
                  name: 'Material Status',
                  key: 'material_status',
                  dataType: {
                    name: 'MasterData',
                    subType: 'MasterDataId',
                    values: {status: 'fetched',values:['Active', 'Not Approved']}
                  },
                  editable: true,
                  validity: {isValid: true, msg: ''},
                  value: 'Active'
                }
              ]
            }
          ]
        };
        wrapper = shallow(<EditComponent {...props}/>);
        const message = 'Changing Material Status will impact existing purchase orders in Fugue. Existing purchase orders need to be closed before the Material Status is updated.';
        wrapper.find('Component').simulate('columnChange', 'general', 'material_status', 'Not Approved','Material Status');
        expect(wrapper).to.have.descendants('AlertDialog');
        wrapper.find('AlertDialog').simulate('ok');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.deep.equal(message);
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
      });
    });

    context('when component type is service', () => {
      let wrapper;
      beforeEach(() => {
        props.componentType = 'service';
        props.componentCode = 'FDP0001';
        props.dependencyDefinitions = {
          serviceClassifications: {
            isFetching: false,
            values: {
              columnList: ["Service Level 1", "Service Level 2"],
              block: {
                name: 'Dependency',
                children: [{
                  name: 'Civil Works',
                  children: [{
                    name: 'Earthwork',
                    children: []
                  }]
                },
                  {name: 'FLOORING | DADO | PAVIOUR',
                    children: [{
                      name:'Flooring',
                      children: []
                    },
                      {
                        name:'Different',
                        children: []
                      }]
                  }]
              }
            },
          }
        };
        props.details = { serviceDetails:
          {
            headers: [
              {
                columns:[
                  {
                    name: 'Service Level 1',
                    key: 'service_level_1',
                    value: "FLOORING | DADO | PAVIOUR",
                    isRequired: false,
                    dataType: {
                      name: "Constant",
                      subType: "FLOORING | DADO | PAVIOUR"
                    }
                  },
                  {
                    name: 'Service Level 2',
                    key: 'service_level_2',
                    value: "Flooring",
                    isRequired: false,
                    dataType: {
                      name: "MasterData",
                      subType: "MasterDataId"
                    }
                  }
                ],
                name: 'Classification',
                key: 'classification'
              },
              {
                columns: [
                  {
                    name: 'Service Code',
                    key: 'service_code',
                    dataType: {
                      name: "Autogenerated",
                      subType: "Service Code"
                    },
                    "value": "FDP0001"
                  },
                  {
                    name: 'WBS Code',
                    key: 'wbs_code',
                    value: null,
                    isRequired: false,
                    dataType: {
                      name: "String",
                      subType: null
                    }
                  },
                  {
                    name: 'Short Description',
                    key: 'short_description',
                    value: "FLOORING | DADO | PAVIOUR-Flooring-Natural Stone-Kota Blue",
                    isRequired: true,
                    dataType: {
                      name: "String",
                      subType: null
                    }
                  },
                  {
                    name: 'Unit of Measure',
                    key: 'unit_of_measure',
                    value: "m2",
                    isRequired: true,
                    dataType: {
                      name: "String",
                      subType: null
                    }
                  },
                  {
                    name: 'HSN Code',
                    key: 'hsn_code',
                    value: null,
                    isRequired: false,
                    dataType: {
                      name: "String",
                      subType: null
                    }
                  },
                  {
                    name: 'Service Status',
                    key: 'service_status',
                    value: "Approved",
                    isRequired: true,
                    dataType: {
                      name: "MasterData",
                      subType: "58a18468c40d555370dd73e6",
                      values: {status: 'fetched',values:['Active', 'Not Approved']}
                    }
                  }
                ],
                name: 'General',
                key: 'general'
              }
            ]
          },classificationDefinition: undefined};
        wrapper = shallow(<EditComponent {...props}/>);
      });

      it('component should have the prop dependencyDefinition for corresponding definition type', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper.find('Component')).to.have.prop('dependencyDefinition')
          .to.deep.equal(props.dependencyDefinitions['serviceClassifications']);
      });

      it('should render title of component on basis of component type', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.include.text('FDP0001');
      });

      it('should show warning message when service status is changed', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const message = 'Changing Service Status will impact existing service orders in Fugue. Existing service orders need to be closed before the Service Status is updated.' +
          '';
        wrapper.find('Component').simulate('columnChange', 'general', 'service_status', 'Not Approved','Service Status');
        expect(wrapper).to.have.descendants('AlertDialog');
        wrapper.find('AlertDialog').simulate('ok');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.deep.equal(message);
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onMasterDataFetch,', () => {
        const onMasterDataFetchSpy = sinon.spy();
        props.onMasterDataFetch = onMasterDataFetchSpy;
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('masterDataFetch');
        expect(onMasterDataFetchSpy).to.be.called;
      });

      it('should render form', () => {
        expect(wrapper).to.have.descendants('form');
      });

      it('should call onUpdateService when form is submitted', () => {
        props.componentCode = 'FDP0001';
        props.details.serviceDetails = {
          headers: [
            {
              columns:[
                {
                  name: 'Service Level 1',
                  key: 'service_level_1',
                  value: "FLOORING | DADO | PAVIOUR",
                  isRequired: false,
                  dataType: {
                    name: "Constant",
                    subType: "FLOORING | DADO | PAVIOUR"
                  }
                },
                {
                  name: 'Service Level 2',
                  key: 'service_level_2',
                  value: "Flooring",
                  isRequired: false,
                  dataType: {
                    name: "MasterData",
                    subType: "MasterDataId"
                  }
                }
              ],
              name: 'Classification',
              key: 'classification'
            },
          ]
        };
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('Component').simulate('columnChange', 'classification', 'service_level_1', 'FLOORING | DADO | PAVIOUR','Service Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'service_level_2', 'Different', 'Service Level 2');
        wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
        const expected = {
          headers: [
            {
              columns:[
                {
                  name: 'Service Level 1',
                  key: 'service_level_1',
                  value: "FLOORING | DADO | PAVIOUR",
                  isRequired: false,
                  dataType: {
                    name: "Constant",
                    subType: "FLOORING | DADO | PAVIOUR"
                  },
                  editable:true,
                  validity: {
                    isValid: true,
                    msg: ""
                  },
                },
                {
                  name: 'Service Level 2',
                  key: 'service_level_2',
                  value: "Different",
                  isRequired: false,
                  dataType: {
                    name: "MasterData",
                    subType: "MasterDataId"
                  },
                  editable:true,
                  validity: {
                    isValid: true,
                    msg: ""
                  },
                }
              ],
              name: 'Classification',
              key: 'classification'
            },
          ]
        };
        expect(props.onUpdateService).to.have.been.calledWith('FDP0001',expected);
        expect(preventDefaultSpy).to.have.been.called;
      });

      it('should not call onUpdateService and show an alert when dependency definition does not match', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'classification', 'service_level_1', 'FLOORING | DADO | PAVIOUR','Service Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'service_level_2', 'New','Service Level 2');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onUpdateService).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should disable update when updating is true', () => {
        props.componentUpdating = true;
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper.find('input[type="submit"]')).to.have.prop('disabled');
      });

      it('should not call onUpdateService and show an alert when required column has null value', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'general', 'unit_of_measure', '','Unit of Measure');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onUpdateService).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should render ConfirmationDialog when cancel button is clicked', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('message').to.equal('The changes made to Service FDP0001 will not be saved. Do you wish to continue?');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onCancel when cancel button is clicked and yes button is clicked on ConfirmationDialog', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('yes');
        expect(props.onCancelUpdateService).to.have.been.calledWith('FDP0001');
      });

      it('should not call onCancel when cancel button is clicked and no button is clicked on ConfirmationDialog', () => {
        wrapper = shallow(<EditComponent {...props}/>);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('#cancel-button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.be.called;
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('no');
        expect(props.onCancelUpdateService).to.be.not.called;
      });

      it('should render AlertDialog with message when componentCreateError is passed', () => {
        props.componentUpdateError = {message:'Error'};
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.equal('Error');
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onResetError when componentCreateError is passed and click ok on alertDialog', () => {
        props.componentUpdateError = {message:'Error'};
        wrapper = shallow(<EditComponent {...props}/>);
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.equal('Error');
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
        wrapper.find('AlertDialog').simulate('close');
        expect(props.onResetError).to.be.called;
      });

      it('should show warning message when unit of measure is changed', () => {
        const message = 'Changing Unit of Measure will impact existing service orders in Fugue. Existing service orders need to be closed before the Unit of Measure is updated.';
        wrapper = shallow(<EditComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'general', 'unit_of_measure', 'MTT','Unit of Measure');
        expect(wrapper).to.have.descendants('AlertDialog');
        wrapper.find('AlertDialog').simulate('ok');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.deep.equal(message);
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;

      });
    });
  });
});
