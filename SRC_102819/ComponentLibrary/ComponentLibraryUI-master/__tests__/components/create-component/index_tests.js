import React from 'react';
import {shallow} from 'enzyme';
import {CreateComponent} from '../../../src/components/create-component';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('CreateComponent', () => {
  let props, onMaterialDefinitionFetchSpy, onDependencyDefintionFetchSpy, createComponent;
  beforeEach(() => {
    onMaterialDefinitionFetchSpy = sinon.spy();
    onDependencyDefintionFetchSpy = sinon.spy();
    props = {
      onMaterialDefinitionFetch: onMaterialDefinitionFetchSpy,
      onDependencyDefinitionFetch: onDependencyDefintionFetchSpy,
      componentType: 'material',
      dependencyDefinitions: {},
      componentCreateError: {message: ''},
      dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''}
    };
  });
  describe('componentDidMount', () => {
    it('should call onMaterialDefinitionFetch when definition is not present and componentType is material', () => {
      new CreateComponent(props).componentDidMount();
      expect(onDependencyDefintionFetchSpy).to.have.been.called;
    });

    it('should call onDependencyDefinitionFetch when dependencyDefinition is not present and componentType is material', () => {
      new CreateComponent(props).componentDidMount();
      expect(onDependencyDefintionFetchSpy).to.have.been.calledWith('materialClassifications');
    });
  });

  describe('componentWillReceiveProps', () => {
    it('should update error', () => {
      const props = {
        dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''},
        definitions: {
          'Clay Material': {
            headers: [
              {
                name: 'Classification',
                columns: [
                  {
                    name: 'Material Level 1',
                    dataType: {
                      name: 'MasterData',
                      subType: 'MasterDataId'
                    }
                  }
                ]
              }
            ]
          }
        },
        masterData: {},
        onColumnChange: sinon.spy(),
        dependencyDefinitions: {
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
                }]
              }
            },
          }
        },
        onAddMaterial: sinon.spy(),
        componentType: 'material',
        onCancelMaterial: sinon.spy(),
        componentCreateError: {message: ''}
      };
      const nextProps = Object.assign({}, props, {componentCreateError: {message: 'Message'}})
      const wrapper = new CreateComponent(props);
      sinon.spy(wrapper, 'setState');
      wrapper.componentWillReceiveProps(nextProps);
      expect(wrapper.setState).to.have.been.calledWith({error: {message: 'Message', shown: true}});
    });
    it('should update error', () => {
      const props = {
        dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''},
        definitions: {
          'Clay Material': {
            headers: [
              {
                name: 'Classification',
                columns: [
                  {
                    name: 'Material Level 1',
                    dataType: {
                      name: 'MasterData',
                      subType: 'MasterDataId'
                    }
                  }
                ]
              }
            ]
          }
        },
        masterData: {},
        onColumnChange: sinon.spy(),
        dependencyDefinitions: {
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
                }]
              }
            },
          }
        },
        onAddMaterial: sinon.spy(),
        componentType: 'material',
        onCancelMaterial: sinon.spy(),
        componentCreateError: {message: 'Message'}
      };
      const wrapper = new CreateComponent(props);
      sinon.spy(wrapper, 'setState');
      wrapper.componentWillReceiveProps(props);
      expect(wrapper.setState).to.not.have.been.calledWith({error: {message: 'Message', shown: true}});
    });
  });

  describe('render', () => {
    it('should show Loading when dependency is not present', () => {
      expect(shallow(<CreateComponent {...props}/>)).to.have.descendants('Loading');
    });
    context('when component data is present', () => {
      let wrapper;
      beforeEach(() => {
        props = {
          dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''},
          definitions: {
            'Clay Material': {
              headers: [
                {
                  name: 'Classification',
                  key: 'classification',
                  columns: [
                    {
                      name: 'Material Level 1',
                      key: 'material_level_1',
                      dataType: {
                        name: 'MasterData',
                        subType: 'MasterDataId'
                      }
                    }
                  ]
                }
              ]
            }
          },
          masterData: {},
          onColumnChange: sinon.spy(),
          dependencyDefinitions: {
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
                  }]
                }
              },
            }
          },
          onAddMaterial: sinon.spy(),
          componentType: 'material',
          onCancelMaterial: sinon.spy(),
          componentCreateError: {message: ''}
        };
        wrapper = shallow(<CreateComponent {...props}/>);
      });

      it('should show loading when isFetching is true', () => {
        props.dependencyDefinitions.materialClassifications.isFetching = true;
        expect(shallow(<CreateComponent {...props}/>)).to.have.descendants('Loading');
      });
      it('should render Create New Material title', () => {
        expect(wrapper).to.include.text('Create New Material');
      });

      it('should render Component', () => {
        expect(wrapper).to.have.descendants('Component');
        const expectedDetails = {
          Classification: {
            'Material Level 1': {
              dataType: {
                name: 'MasterData',
                subType: 'MasterDataId'
              },
              value: '',
              editable: true,
            }
          }
        };

        expect(wrapper.find('Component')).to.have.prop('masterData').deep.equal({});
        expect(wrapper.find('Component')).to.have.prop('editable').to.be.true;
      });

      it('should call onMasterDataFetch,', () => {
        const onMasterDataFetchSpy = sinon.spy();
        props.onMasterDataFetch = onMasterDataFetchSpy;
        wrapper = shallow(<CreateComponent {...props}/>);
        wrapper.find('Component').simulate('masterDataFetch');
        expect(onMasterDataFetchSpy).to.be.called;
      });

      it('should render show form', () => {
        expect(wrapper).to.have.descendants('form');
      });

      it('should call onAddMaterial when form is submitted', () => {
        const preventDefaultSpy = sinon.spy();
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_2', 'Clay Material','Material Level 2');
        wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
        const expected = {
          headers: [
            {
              columns: [
                {
                  key: 'material_level_1',
                  name: 'Material Level 1',
                  dataType: {
                    name: "MasterData",
                    values: {
                      status: "fetched",
                      values : []
                    },
                    value:""
                  },
                  editable: true,
                  validity: { isValid: true, msg: "" },
                  value:'Primary'
                },
                {
                  name: 'Material Level 2',
                  key: 'material_level_2',
                  dataType: {
                    name: "MasterData",
                    values: {
                      status: "fetched",
                      values: []
                    },
                    value:""
                  },
                  editable: true,
                  validity: { isValid: true, msg: "" },
                  value:'Clay Material'
                }
              ],
              name: 'Classification',
              key: 'classification'
            }
          ]
        };
        expect(props.onAddMaterial).to.have.been.calledWith(expected);
        expect(preventDefaultSpy).to.have.been.called;
      });

      it('should not call onAddMaterial and show an alert when dependency definition does not match', () => {
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_2', '','Material Level 2');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onAddMaterial).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should disable add when adding is true', () => {
        props.componentAdding = true;
        wrapper = shallow(<CreateComponent {...props}/>);
        expect(wrapper.find('input[type="submit"]')).to.have.prop('disabled');
      });

      it('should not call onAddMaterial and show an alert when data is invalid', () => {
        props = {
          dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''},
          definitions: {
            'Clay Material': {
              headers: [
                {
                  name: 'Classification',
                  key: 'classification',
                  columns: [
                    {
                      name: 'Material Level 1',
                      key: 'material_level_1',
                      dataType: {
                        name: 'MasterData',
                        subType: 'MasterDataId'
                      }
                    }
                  ]
                },
                {
                  name: 'Purchase',
                  key: 'purchase',
                  columns: [
                    {
                      name: 'Last Purchase Rate',
                      key: 'last_purchase_rate',
                      dataType: {
                        name: 'Money',
                        subType: 'null'
                      }
                    }
                  ]
                }
              ]
            }
          },
          masterData: {},
          onColumnChange: sinon.spy(),
          dependencyDefinitions: {
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
                  }]
                }
              },
            }
          },
          onAddMaterial: sinon.spy(),
          componentType: 'material',
          onCancelMaterial: sinon.spy(),
          componentCreateError: {message: ''},
        };
        wrapper = shallow(<CreateComponent {...props}/>);
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_1', 'Primary','Material Level 1');
        wrapper.find('Component').simulate('columnChange', 'classification', 'material_level_2', 'Clay Material','Material Level 2');
        wrapper.find('Component').simulate('columnChange', 'purchase', 'last_purchase_rate', {amount: '3.3.3.3', currency: 'INR'},'Last Purchase Rate');
        wrapper.find('form').simulate('submit', {preventDefault: sinon.spy()});
        expect(props.onAddMaterial).to.not.have.been.called;
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message');
      });

      it('should render ConfirmationDialog when cancel button is clicked', () => {
        wrapper.find('#cancel-button').simulate('click');
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('message').to.equal('The material will not be created. Do you wish to continue?');
        expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.true;
      });

      it('should call onCancel when cancel button is clicked and yes button is clicked on ConfirmationDialog', () => {
        wrapper.find('#cancel-button').simulate('click');
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('yes');
        expect(props.onCancelMaterial).to.be.called;
      });

      it('should not call onCancel when cancel button is clicked and no button is clicked on ConfirmationDialog', () => {
        wrapper.find('#cancel-button').simulate('click');
        expect(wrapper).to.have.descendants('ConfirmationDialog');
        wrapper.find('ConfirmationDialog').simulate('no');
        expect(props.onCancelMaterial).to.be.not.called;
      });

      it('should render AlertDialog with message when componentCreateError is passed', () => {
        props.componentCreateError = {message: 'Error'};
        const wrapper = shallow(<CreateComponent {...props}/>);
        expect(wrapper).to.have.descendants('AlertDialog');
        expect(wrapper.find('AlertDialog')).to.have.prop('message').to.equal('Error');
        expect(wrapper.find('AlertDialog')).to.have.prop('shown').to.be.true;
      });

      it('should render the component initially with empty details', () => {
        props = {
          dependencyDefinitionError: {material: '', sfg: '', service: '', package: ''},
          definition: {
            headers: [
              {
                name: 'Classification',
                key: 'classification',
                columns: [
                  {
                    name: 'Material Level 1',
                    key: 'material_level_1',
                    dataType: {
                      name: 'MasterData',
                      subType: 'MasterDataId'
                    },
                    value: ''
                  },
                  {
                    name: 'Material Level 2',
                    key: 'material_level_2',
                    dataType: {
                      name: 'MasterData',
                      subType: 'MasterDataId'
                    },
                    value: ''
                  }
                ]
              }
            ]
          },
          dependencyDefinitions: {materialClassifications: {values: {columnList: []}, isFetching: false}},
          componentCreateError: {message: ''},
          componentType: 'material'
        };
        wrapper = shallow(<CreateComponent {...props}/>);

        const expected = {
          headers: [
            {
              columns:[],
              key: 'classification',
              name: 'Classification'
            }
          ]
        };
        expect(wrapper.find('Component')).to.have.prop('details').deep.equal(expected);
      });
    });
  });
});
