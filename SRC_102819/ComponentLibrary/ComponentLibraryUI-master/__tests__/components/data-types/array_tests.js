import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Array, __RewireAPI__ as ArrayRewired} from '../../../src/components/data-types/array';
import  DataType from '../../../src/components/data-types';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Array', () => {
  describe('render', () => {
    it('should render the DataType for all children', () => {
      const props = {
        columnName: "Image", columnValue: {
          "value": [
            "BMB00001",
            "BMB00002"
          ],
          "dataType": {
            "name": "Array",
            "subType": {
              name: "String",
              subType: ""
            }
          }
        },
        componentType: "material"
      };
      const wrapper = shallow(<Array {...props} />);
      expect(wrapper).to.have.exactly(2).descendants('DataType');
      expect(wrapper).to.have.descendants('ReadMore');
    });
    context('editable is true', () => {
      let props, wrapper;
      beforeEach(() => {
        props = {
          columnName: "Image",
          columnValue: {
            value: ["Sattar"],
            editable: true,
            dataType: {
              name: "Array",
              subType: {
                name: "String",
                subType: ""
              }
            }
          },
          onChange: sinon.spy(),
          componentType: "material"
        };
        ArrayRewired.__Rewire__('styles', {addAnother: 'add-another', delete: 'delete'});
        wrapper = shallow(<Array {...props} />);
      });
      it('should render Add another button', () => {
        expect(wrapper).to.have.descendants('button.add-another');
      });

      it('should add a value if Add Another is clicked', () => {
        const preventDefaultSpy = sinon.spy();
        wrapper.find('button.add-another').simulate('click', {preventDefault: preventDefaultSpy});
        expect(props.onChange).to.have.been.calledWith(["Sattar", null]);
        expect(preventDefaultSpy).to.have.been.called;
      });
      it('should add a value if Add Another is clicked and existing value is null', () => {
        const preventDefaultSpy = sinon.spy();
        props.columnValue.value = null;
        wrapper = shallow(<Array {...props} />);
        wrapper.find('button').simulate('click', {preventDefault: preventDefaultSpy});
        expect(props.onChange).to.have.been.calledWith([null]);
        expect(preventDefaultSpy).to.have.been.called;
      });

      it('should display as many datatypes as there are values', () => {
        const preventDefaultSpy = sinon.spy();
        props.columnValue.value = ["Sattar", "Jishnu"];
        wrapper = shallow(<Array {...props} />);
        expect(wrapper).have.exactly(2).descendants('DataType');
      });

      it('should call onChange when a dataType calls onChange with value', () => {
        wrapper.find('DataType').simulate('change', 'Jishnu');
        expect(props.onChange).to.have.been.calledWith(['Jishnu']);
      });
      it('should render delete button after datatype', () => {
        expect(wrapper).to.have.descendants('button.delete');
      });
      it('should call onChange with value removed if the minus button is clicked', () => {
        props.columnValue.value = ["Sattar", "Abdul"];
        wrapper = shallow(<Array {...props} />);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('button.delete').first().simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.have.been.called;
        expect(props.onChange).to.have.been.calledWith(['Abdul']);
      });
      it('should call onChange with null if the last element is removed', () => {
        props.columnValue.value = ["Sattar"];
        wrapper = shallow(<Array {...props} />);
        const preventDefaultSpy = sinon.spy();
        wrapper.find('button.delete').simulate('click', {preventDefault: preventDefaultSpy});
        expect(preventDefaultSpy).to.have.been.called;
        expect(props.onChange).to.have.been.calledWith(null);
      });

      context('when filterable and editable is true', () => {
        beforeEach(() => {
          props.columnValue.filterable = true;
        });
        it("should render Data type whose type is string when array subtype is string", () => {
          props.columnValue.value = "value";
          wrapper = shallow(<Array {...props} />);
          expect(wrapper.find('DataType')).to.have.prop('columnValue').deep.equal({value: "value",
            filterable:true,
            editable:true,
            dataType: {
              name: "String",
              subType:""
            }});
        });

        it('should call on change of props when onChange of DataType is triggered', () => {
          props.columnValue.value = "value";
          wrapper = shallow(<Array {...props} />);
          wrapper.find('DataType').simulate('change');
          expect(props.onChange).to.have.been.called;
        });
      });
    });
  });
});
